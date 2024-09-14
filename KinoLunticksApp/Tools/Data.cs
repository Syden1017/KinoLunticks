using KinoLunticksApp.Models;

namespace KinoLunticksApp.Tools
{
    class Data
    {
        private static User? user;

        // Признаки авторизации
        public static bool Login = false;
        public static string UserSurname;
        public static string UserName;

        // Права доступа
        public static string AccessRights;
    }
}