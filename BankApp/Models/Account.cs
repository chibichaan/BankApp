namespace BankApp.Models;

public class Account
{
    //номер счета, дата создания счета, депозит, кредитные средства overdrow, индитификатор Guif, индитиф CustomerId
    public Guid Id { get; set; }
    public string Number { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreationDate { get; set; }
    public decimal Deposit { get; set; } //баланс
    public decimal Overdrow { get; set; }
}