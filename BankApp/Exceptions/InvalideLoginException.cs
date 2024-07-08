namespace BankApp.Exceptions;

public class InvalideLoginException : Exception
{
    public InvalideLoginException(){}
    public InvalideLoginException(string message) : base(message) { }
}