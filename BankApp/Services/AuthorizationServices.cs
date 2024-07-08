using System.Security.Cryptography;
using System.Text;
using BankApp.Exceptions;
using BankApp.Models;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BankApp.Services;

public class AuthorizationServices
{
    private const string FILE_NAME = "customers.json";
    
    //доступ пользователя по логин/пароль, 
    public (bool isSuccesses, Guid customerId) Login(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new InvalideLoginException();
        }

        AuthorizationGuard.CheckPassword(password);
        
        if (!File.Exists(FILE_NAME))
        {
            throw new DataBaseNotFoundException("Отсутствует соединение с базой данных.");
        }
        
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Login == login);
        
        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с логином {login} не найден в системе");
        }

        var hash = GetMD5(password);
        return (customer.Password == hash, customer.Id);
    }
    
    public void ChangePassword(string login, string newPassword)
    {
        AuthorizationGuard.CheckPassword(newPassword); // проверяем новый пароль
        
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

}