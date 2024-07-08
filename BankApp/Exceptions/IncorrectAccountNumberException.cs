namespace BankApp.Exceptions;

public class IncorrectAccountNumberException : ArgumentException
{
    public IncorrectAccountNumberException (string message) : base(message) { }
}