namespace BankApp.Exceptions;

public class SumOfCreditException : ArgumentException
{
    public SumOfCreditException (string message) : base(message) { }
}