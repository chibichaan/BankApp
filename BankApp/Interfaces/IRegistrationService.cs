namespace BankApp.Interfaces;

public interface IRegistrationService
{
    void Register(string login, string password, string fName, string lName, string mName);
    void ChangePassword(string login, string newPassword);
}