namespace BankApp.Exceptions;

public class InvalideSurnameException : Exception
{
    public InvalideSurnameException(string message) : base(message) { }
}