namespace BankApp.Exceptions;

public class IncorrectPasswordException : ArgumentException
{
    public IncorrectPasswordException() {}
    public IncorrectPasswordException(string message) : base(message) { }
}