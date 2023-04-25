using System.Net;
using System.Text.RegularExpressions;

namespace PracticalWork6
{
    // Класс валидации для проверки правильности ввода имени пользователя и IP адреса
    public static class Validation
    {
        public static bool IsValidUsername(string username)
        {
            // Проверка имени пользователя на длину и наличие только букв, цифр и знака подчеркивания
            return !string.IsNullOrEmpty(username) && Regex.IsMatch(username, @"^\w+$");
        }

        public static bool IsValidIP(string ip)
        {
            // Проверка IP-адреса на корректность
            IPAddress address;
            return IPAddress.TryParse(ip, out address);
        }
    }
}
