namespace BankApp;

public class ClosingCreditException : ArgumentException
{
    public ClosingCreditException (string message) : base(message) { }
}