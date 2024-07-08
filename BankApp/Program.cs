using BankApp;
using BankApp.Exceptions;
using BankApp.Models;
using BankApp.Services;


bool showMenu = true;
while (showMenu)
{
    showMenu = MainMenu();
}

static bool MainMenu()
{
    Console.WriteLine("Ознакомьтесь с несколькими возможными действиями:");
    Console.WriteLine("1 - Регистрация");
    Console.WriteLine("2 - Авторизация ");
    Console.WriteLine("3 - Выход из приложения");
    Console.Write("\r\nВведите одну из цифр: ");
 
    switch (Console.ReadLine())
    {
        case "1":
            Console.Clear();
            ToRegister();
            return true;
        case "2":
            Console.Clear();
            var authorizationResult = ToLogIn();
            if (authorizationResult.isSuccesses)
            {
                Console.WriteLine("Вы перешли в меню авторизированного пользователя!");
                bool showAuthorizationMenu = true;
                while (showAuthorizationMenu)
                {
                    showAuthorizationMenu = AuthorizationMenu(authorizationResult.customerId.Value);
                }
                return true;
            }
            Console.WriteLine("Не удалось авторизоваться");
            return true;
        case "3":
            Console.Clear();
            Console.WriteLine("Спасибо, что выбрали наше приложение!");
            return false;
        default:
            Console.Clear();
            Console.WriteLine("Неккоректный ввод");
            return true;
    }
}

static bool AuthorizationMenu(Guid customerId)
{
    Console.WriteLine("Ознакомьтесь с несколькими возможными действиями:");
    Console.WriteLine("1 - Создать счёт");
    Console.WriteLine("2 - Закрыть счёт");
    Console.WriteLine("3 - Пополнить счёт");
    Console.WriteLine("4 - Снять со счёта");
    Console.WriteLine("5 - Вывести информацию по конкретному счёту");
    Console.WriteLine("6 - Вывести информацию по всем счетам");
    Console.WriteLine("7 - Взять кредит");
    Console.WriteLine("8 - Вернуться в главное меню");
    Console.Write("\r\nВведите одну из цифр: ");
    
    switch (Console.ReadLine())
    {
        case "1":
            Console.Clear();
            ToCreateAccount(customerId);
            return true;
        case "2":       
            Console.Clear();
            ToClosingAccount(customerId);
            return true;
        case "3": 
            Console.Clear();
            ToTopUpAccount(customerId); //пополнить счёт
            return false;
        case "4": 
            Console.Clear();
            ToWithdrawAccount(customerId); //снять со счета
            return false;
        case "5": 
            Console.Clear();
            ToOutPutAccountByNumber(customerId);
            return false;
        case "6": 
            Console.Clear();
            ToOutPutAllAccountsNumbers(customerId);
            return false;
        case "7": 
            Console.Clear();
            ToTakeACredit(customerId);
            return false;
        case "8":
            Console.Clear();
            Console.WriteLine("Вы перешли в главное меню!");
            return false;
        default:
            Console.Clear();
            Console.WriteLine("Неккоректный ввод");
            return true;
    }
}

static void ToOutPutAllAccountsNumbers(Guid customerId)
{
    var acInfo = new AccountServices();
            
    try
    {
        acInfo.OutPutAllAccountsNumbers(customerId);
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (NumberNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
}

static void ToOutPutAccountByNumber(Guid customerId)
{
    var accInfo = new AccountServices();
    var accNumber = GetUserInput("Введите номер счета, который хотите посмотреть");

    try
    {
        accInfo.OutPutAccountByNumber(customerId, accNumber);    
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (IncorrectAccountNumberException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (NumberNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
}

static void ToTakeACredit(Guid customerId)
{
    var acInf = new AccountServices();
            
    var accountNumber = GetUserInput("Введите номер счета, на который хотите взять кредит");
    Console.WriteLine("Введите сумму кредита. Она не должна превышать 30000");
    var creditSum = Convert.ToDecimal(Console.ReadLine());
    try
    {
        acInf.TakeACredit(customerId, accountNumber, creditSum);
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (SumOfCreditException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (IncorrectAccountNumberException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (NumberNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (SumOdCreditIsNotNullException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (IncorrectEnteredMoneyException e)
    {
        Console.WriteLine(e.Message);
    }
}

static bool ToCreateAccount(Guid customerId)
{
    var accountServ = new AccountServices();
    try
    {
        var accountCreated = accountServ.CreateAccount(customerId);
        return accountCreated;
    }
    catch (FileNameException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    return true;
}

static bool ToClosingAccount(Guid customerId)
{
    var accountNumber = GetUserInput("Введите номер счета, на который хотите положить деньги");
            
    var accServ = new AccountServices();
    try
    {
        var accountDeleted = accServ.ClosingAccount(customerId, accountNumber);
        return accountDeleted;
    }
    catch (FileNameException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (IncorrectAccountNumberException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (NumberNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (ClosingCreditException e)
    {
        Console.WriteLine(e.Message);
    }

    return true;
}

static void ToRegister()
{
    var regServ = new RegistrationService();
    var inputLogin = GetUserInput("Введите логин");
    var inputPassword = GetUserInput("Введите пароль");
    var inputName = GetUserInput("Введите имя");
    var inputSurname = GetUserInput("Введите фамилию");
    var inputMiddleName = GetUserInput("Введите отчество");

    try
    {
        regServ.Register(inputLogin, inputPassword, inputName, inputSurname, inputMiddleName);
    }
    catch (FileNotFoundException e)
    {
        Console.WriteLine(e.Message);
        File.WriteAllBytes("customers.json", Array.Empty<byte>());
        Console.WriteLine("Был создан файл");
        regServ.Register(inputLogin, inputPassword, inputName, inputSurname, inputMiddleName);
    }
    catch (DuplicateLoginException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (IncorrectPasswordException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (InvalideLoginException e) 
    {
        Console.WriteLine(e.Message);
    }
    catch (InvalideNameException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (InvalideSurnameException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (InvalidePatronymicException e)
    {
        Console.WriteLine(e.Message);
    }
}

static (bool isSuccesses, Guid? customerId) ToLogIn()
{
    var authServ = new AuthorizationServices();
    var result = (isSuccesses: false, customerId: default (Guid?));
                                                  // или (Guid?)null
    var inputYourLogin = GetUserInput("Введите логин"); 
    var inputPassword = GetUserInput("Введите пароль");
    try
    {
       result = authServ.Login(inputYourLogin, inputPassword);
    }
    catch (IncorrectPasswordException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (DataBaseNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    return result;
}

static void ToTopUpAccount(Guid customerId)
{
    var accountNumber = GetUserInput("Введите номер счета, на который хотите положить деньги");
    Console.WriteLine("Введите нужную сумму денег в рублях");
    var money = Convert.ToDecimal(Console.ReadLine());
            
    var topUpAccServ = new AccountServices();
    try
    {
        topUpAccServ.TopUpAccount(customerId, accountNumber, money);
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (IncorrectEnteredMoneyException e)
    {
        Console.WriteLine(e.Message);
    }
}

static void ToWithdrawAccount(Guid customerId)
{
    var accountNumber = GetUserInput("Введите номер счета, с которого хотите снять деньги");
    Console.WriteLine("Введите нужную сумму денег в рублях");
    var money = Convert.ToDecimal(Console.ReadLine());
            
    var withdrawAccServ = new AccountServices();
    try
    {
        withdrawAccServ.WithdrawAccount(customerId, accountNumber, money);
    }
    catch (UserNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (CashWithdrawalException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (IncorrectEnteredMoneyException e)
    {
        Console.WriteLine(e.Message);
    }
}

static string? GetUserInput(string message) //  ? - строка может быть nullable
{
    Console.WriteLine(message);
    return Console.ReadLine();
}
