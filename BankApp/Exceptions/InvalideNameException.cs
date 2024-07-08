namespace BankApp.Exceptions;

public class InvalideNameException :Exception
{
    public InvalideNameException(string message) : base(message) { }
}