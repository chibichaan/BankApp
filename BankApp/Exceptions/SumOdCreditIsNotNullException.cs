namespace BankApp.Exceptions;

public class SumOdCreditIsNotNullException : ArgumentException
{
    public SumOdCreditIsNotNullException (string message) : base(message){ }
}