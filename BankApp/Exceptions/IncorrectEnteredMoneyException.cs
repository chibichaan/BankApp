namespace BankApp.Exceptions;

public class IncorrectEnteredMoneyException : ArgumentException
{
    public IncorrectEnteredMoneyException(string message) : base(message) { }
}