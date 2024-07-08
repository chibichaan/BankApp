namespace BankApp.Exceptions;

public class DataBaseNotFoundException : Exception
{
    public DataBaseNotFoundException(string message) : base(message) { }
}