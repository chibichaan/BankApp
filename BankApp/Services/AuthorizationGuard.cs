using System.Text.RegularExpressions;
using BankApp.Exceptions;

namespace BankApp.Services;

public static class AuthorizationGuard
{
    /// <summary>
    /// Проверка пароля
    /// </summary>
    /// <param name="password">вводимый пароль</param>
    /// <exception cref="IncorrectPasswordException">Ошибка неверного пароля</exception>
    public static void CheckPassword(string password)
    {
        //онлайн работа с регул. выжарениями reg ex online
        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasLowerChar = new Regex(@"[a-z]+");
        //var hasMinimum8Chars = new Regex(@".{8,}");
       
        // если пароль не проходит данные требования, то выбрасывается исключение
        if (password.Length <= 7  ||
            !hasNumber.IsMatch(password) ||
            !hasUpperChar.IsMatch(password) ||
            !hasLowerChar.IsMatch(password)
           )
        {
            throw new IncorrectPasswordException($"Пароль {password} очень легкий :(");
        }
    }
}