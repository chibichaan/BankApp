using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.RegularExpressions;
using BankApp.Exceptions;
using BankApp.Models;
using Newtonsoft.Json;

namespace BankApp.Services;

public class AccountServices
{
    //снятие/зачисление денег со счета
    //создание счета
    private const string FILE_NAME = "customers.json";
    private const decimal MAX_CREDIT_SUM = 30000;
    
    public bool CreateAccount(Guid customerId)
    {
        //обратиться в файл, достать пользователя, ему добавить в его коллекцию новый созданный аккаунт 
        //затем полностью сохранить весь список пользователей
        if (!File.Exists(FILE_NAME))
        {
            throw new FileNameException("Такого файла не существует. :( ");
        }
        
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с идентификатором {customerId} не найден в системе");
        }

        var uniqNumber = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()))
            .Substring(0, 4);

        var newAccount = new Account
        {
            Id = Guid.NewGuid(),
            Number = uniqNumber,
            CustomerId = customerId,
            CreationDate = DateTime.UtcNow,
            Deposit = 0,
            Overdrow = 0
        };
        customer.Accounts.Add(newAccount);
        var jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);
        File.WriteAllText("customers.json",jsonString);
        Console.WriteLine($"Был успешно создан аккаунт с индентификатором {newAccount.Number}");
        return true;
    }

    //при закрытии счета(у нас удаление, переименовать) проверка что нет кредитных средств
    //при становлении кредита ноль - закрытие кредита - поздравить в консоль
    public bool ClosingAccount(Guid customerId, string? accountNumber)
    {
        if (!File.Exists(FILE_NAME))
        {
            throw new FileNameException("Такого файла не существует. :( ");
        }
        
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с идентификатором {customerId} не найден в системе");
        }

        if (accountNumber == null || accountNumber.Length != 4)
        {
            throw new IncorrectAccountNumberException("Введенный неверный номер счета аакаунта.");
        }

        var account = customer.Accounts.SingleOrDefault(a => a.Number == accountNumber);
        if (account == null)
        {
            throw new NumberNotFoundException("Данного номера счёта аккаунта не существует. " +
                                              "Проверьте введенный номер.");
        }

        if (account.Overdrow > 0 )
        {
            throw new ClosingCreditException($"Вы не закрыли кредит! " +
                                $"Чтобы закрыть кредит, положите на счёт {account.Overdrow}"); 
        }

        Console.WriteLine("Поздравляем! Вы закрыдли кредит!");
        var jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);
        File.WriteAllText("customers.json",jsonString);
        return true;
    }

    /// <summary>
    /// Пополнить счёт
    /// </summary>
    public void TopUpAccount(Guid customerId, string? accountNumber, decimal addingMoney)
    {
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с идентификатором {customerId} не найден в системе");
        }
        CheckEnteredMoney(addingMoney);
        
        //TODO: Проверка на досточерность номера аккаунта (шутка)
        
        if (accountNumber == null || accountNumber.Length != 4)
        {
            throw new IncorrectAccountNumberException("Введенный неверный номер счета аакаунта.");
        }

        var account = customer.Accounts.SingleOrDefault(a => a.Number == accountNumber);
        if (account == null)
        {
            throw new NumberNotFoundException("Данного номера счёта аккаунта не существует. " +
                                              "Проверьте введенный номер.");
        }
        
        var beforeBalance = account.Deposit;

        var newBalance = beforeBalance + addingMoney;

        account.Deposit = newBalance;
        
        var jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);
        File.WriteAllText("customers.json",jsonString);

    }

    /// <summary>
    /// Снять со счёта
    /// </summary>
    public void WithdrawAccount(Guid customerId, string accountNumber, decimal reqestedMoney)
    {
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с идентификатором {customerId} не найден в системе");
        }
        CheckEnteredMoney(reqestedMoney);
        if (accountNumber == null || accountNumber.Length != 4)
        {
            throw new IncorrectAccountNumberException("Введенный неверный номер счета аакаунта.");
        }

        var account = customer.Accounts.SingleOrDefault(a => a.Number == accountNumber);
        if (account == null)
        {
            throw new NumberNotFoundException("Данного номера счёта аккаунта не существует. " +
                                              "Проверьте введенный номер.");
        }
        
        var beforeBalance = account.Deposit;
        
        if (beforeBalance < reqestedMoney)
        {
            throw new CashWithdrawalException("Вы не можете снять денег больше, чем у вас имеется!");
        }

        var newBalance = beforeBalance - reqestedMoney;

        account.Deposit = newBalance;
        Console.WriteLine($"На вашем счёте осталось {newBalance}");
        
        var jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);
        File.WriteAllText("customers.json",jsonString);
    }
    
    private void CheckEnteredMoney(decimal money)
    {
        if ( money == 0 || money < 0)
        {
            throw new IncorrectEnteredMoneyException($"Ошибка при вводе денег!");
        }
    }

    //TODO: 1) метод выводит весь список счетов пользователя, в формате 
    //"счет номер такой, собственные средства такие то, кредитные средства такие-то"
    public void OutPutAllAccountsNumbers(Guid customerId)
    {
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с идентификатором {customerId} не найден в системе");
        }

        var account = customer.Accounts.FindAll(a => a.Number.Length == 4);
        
         if (account == null)
         {
             throw new NumberNotFoundException("Данного номера счёта аккаунта не существует. " +
                                               "Проверьте введенный номер.");
         }

         for (int i = 0; i < account.Count; i++)
         {
             var number = account[i];
             Console.WriteLine(GetPrintableAccInfo(number));
         }
    }

    // TODO: 2) метод выводит по конкретному счету  (СДЕЛАНО)
    public void OutPutAccountByNumber(Guid customerId, string accountNumber)
    {
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с идентификатором {customerId} не найден в системе");
        }
        
        if (accountNumber == null || accountNumber.Length != 4)
        {
            throw new IncorrectAccountNumberException("Введенный неверный номер счета аакаунта.");
        }

        var account = customer.Accounts.SingleOrDefault(a => a.Number == accountNumber);
        if (account == null)
        {
            throw new NumberNotFoundException("Данного номера счёта аккаунта не существует. " +
                                              "Проверьте введенный номер.");
        }

        Console.WriteLine(GetPrintableAccInfo(account));
    }
    
    private string GetPrintableAccInfo(Account account)
    {
        return $"Счёт № {account.Number}," +
               $" собственные средства: {account.Deposit}," +
               $" кредитные средства: {account.Overdrow}";
    }

    //метод взятия кредита: проверить что кредитный счет ноль, кредит ограничен 30000, 
    //при пополнении счета спрашиваем у пользователя на какой баланс он хочет пополнить
    public void TakeACredit(Guid customerId, string accountNumber, decimal creditSum)
    {
        var json = File.ReadAllText(FILE_NAME);
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json);
        var customer = customers?.FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            throw new UserNotFoundException($"Пользователь с идентификатором {customerId} не найден в системе");
        }
        CheckEnteredMoney(creditSum);
        
        if (creditSum > MAX_CREDIT_SUM) 
        {
            throw new SumOfCreditException("Кредит не может быть больше 30000");
        }
        
        if (accountNumber == null || accountNumber.Length != 4)
        {
            throw new IncorrectAccountNumberException("Введенный неверный номер счета аакаунта.");
        }

        var account = customer.Accounts.SingleOrDefault(a => a.Number == accountNumber);
        if (account == null)
        {
            throw new NumberNotFoundException("Данного номера счёта аккаунта не существует. " +
                                              "Проверьте введенный номер.");
        }
        
        var currentCredit = account.Overdrow;

        if (currentCredit != 0)
        {
            throw new SumOdCreditIsNotNullException("У вас уже есть кредит!"); 
        }

        var newCreditBalance = currentCredit + creditSum;

        account.Overdrow= newCreditBalance;
        Console.WriteLine($"На вашем счёте осталось {newCreditBalance}");
        
        var jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);
        File.WriteAllText("customers.json",jsonString);
    }
    
}