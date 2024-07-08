using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using BankApp.Exceptions;
using BankApp.Interfaces;
using BankApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BankApp.Services;

public class RegistrationService : IRegistrationService
{ 
    private const string FILE_NAME = "customers.json";
    public void Register(string login, string password, string fName, string lName, string mName)
    {
        var customers = new List<Customer>();
        
        //обратиться в файл, достать всех кастомеров, если с таким логином уже есть, то выкидивать исключение, 
        //что пользователь уже зарег-ан,
        if (File.Exists(FILE_NAME))
        {
            var json = File.ReadAllText(FILE_NAME);
            customers = JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
            if (customers.Any(cus => cus.Login == login))
            {
                throw new DuplicateLoginException("Данный логин уже занят. Попробуйте ещё раз.");
            }
        }
        else
        {
            throw new FileNotFoundException($"Данный файл не был найден");
        }
        
        CheckPassword(password);
        
        //почему-то он сюда просто не проходит с точной остановы даже
        //записывает в файл пустые строки! 
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new InvalideLoginException($"Логин не может быть пустым");
        }
        if (string.IsNullOrWhiteSpace(fName))
        {
            throw new InvalideNameException($"Имя не может быть пустым");
        }
        if (string.IsNullOrWhiteSpace(lName))
        {
            throw new InvalideSurnameException($"Фамилия не может быть пустым");
        }
        if (string.IsNullOrWhiteSpace(mName))
        {
            throw new InvalidePatronymicException($"Отчество  не может быть пустым");
        }
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Surname = lName,
            Name = fName,
            Patronymic = mName,
            Login = login,
            Password = GetMD5(password),
            Accounts = new List<Account>()
        };

        // var customer = new Customer();
        // customer.Id = Guid.NewGuid();
        // customer.Surname = lName;
        // customer.Name = fName;
        // customer.Patronymic = mName;
        // customer.Login = login;
        // customer.Password = GetMD5(password);
        // customer.Accounts = new List<Account>();
        
        customers.Add(customer);
        var jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);
        File.WriteAllText("customers.json",jsonString);
        Console.WriteLine("Вы зарегистрировались!");
    }
    
    /// <summary>
    /// Смена пароля
    /// </summary>
    /// <param name="login">вводимый логин</param>
    /// <param name="newPassword">вводимый новый пароль</param>
    /// <exception cref="Exception">ошибка, при которой пользователя нет в базе</exception>
    public void ChangePassword(string login, string newPassword)
    {
        CheckPassword(newPassword); // проверяем новый пароль
        
        if (!File.Exists(FILE_NAME))
        {
            throw new FileNameException("Такого файла не существует. :( ");
        }
        
        //обратиться в файл, достать кастомер по его логину, сравнить хеш у кастомера с новым хешем, если равны, то
        //выводить ошибку, менять пароль у кастомера (перезаписывать весь файл)
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Login == login);
        
        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с логином {login} не найден в системе");
        }

        var hash = GetMD5(newPassword); //вычислить хеш нового пароля
        if (customer.Password == hash)
        {
            throw new SamePasswordsException("Пароли идентичны !!!");
        }

        customer.Password = hash;

        var updateCustomers = JsonSerializer.Serialize(customers);
        File.WriteAllText(FILE_NAME,updateCustomers);
         
    }
    
    /// <summary>
    /// Получить хеш-код пароля
    /// </summary>
    /// <param name="password">вводимый пароль</param>
    /// <returns></returns>
    private string GetMD5(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password.Trim());
        using var md5Hash = MD5.Create();
        var hashBytes = md5Hash.ComputeHash(bytes);
        var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        return hash;
    }
    
    /// <summary>
    /// Проверка пароля 
    /// </summary>
    /// <param name="password">вводимый пароль</param>
    /// <exception cref="IncorrectPasswordException">ошибка легкого пароля</exception>
    private void CheckPassword(string password)
    {
        //онлайн работа с регул. выжарениями reg ex online
       var hasNumber = new Regex(@"[0-9]+");
       var hasUpperChar = new Regex(@"[A-Z]+");
       var hasLowerChar = new Regex(@"[a-z]+");
       
       // если пароль не проходит данные требования, то выбрасывается исключение
       if (password.Length <= 7  &&
           !hasNumber.IsMatch(password) &&
           !hasUpperChar.IsMatch(password) &&
           !hasLowerChar.IsMatch(password)
           )
       {
           throw new IncorrectPasswordException($"Пароль {password} очень легкий :(");
       }
    }
}