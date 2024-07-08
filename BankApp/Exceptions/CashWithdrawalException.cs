namespace BankApp.Exceptions;

public class CashWithdrawalException : ArgumentException
{
    public CashWithdrawalException (string message) : base(message) { }
}