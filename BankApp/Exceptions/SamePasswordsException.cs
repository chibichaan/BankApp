namespace BankApp.Exceptions;

public class SamePasswordsException: ArgumentException
{
    public SamePasswordsException(){}
    public SamePasswordsException(string message) : base(message) { }
}