namespace BankApp.Exceptions;

public class NumberNotFoundException : ArgumentException
{
    public NumberNotFoundException (string message) : base(message) { }
}