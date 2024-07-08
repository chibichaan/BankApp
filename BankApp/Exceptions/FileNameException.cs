namespace BankApp.Exceptions;

public class FileNameException: ArgumentException
{
    public FileNameException(){}
    public FileNameException(string message) : base(message) { }
}