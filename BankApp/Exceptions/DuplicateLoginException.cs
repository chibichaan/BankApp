namespace BankApp.Exceptions;

public class DuplicateLoginException : Exception
{
    public DuplicateLoginException(){}
    public DuplicateLoginException(string message) : base(message) { }
}