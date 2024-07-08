using BankApp.Exceptions;

namespace BankApp.Models;

public class Customer
{
    //фио, список счетов, логин, пароль
    private string surname = "";
    private string name = "";
    private string patronymic = "";
    private string login = "";
    private string password = "";
    public Guid Id { get; set; }

    public string Surname
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public string Patronymic
    {
        get;
        set;
    }

    public string Login
    {
        get;
        set;
    }

    public string Password
    {
        get;
        set;
    }
    public List<Account> Accounts { get; set; }
}