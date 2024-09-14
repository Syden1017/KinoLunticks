using KinoLunticksApp.Models;

namespace KinoLunticksApp.Tools
{
    class UserWork
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        public bool IsValid(string login, string password)
        {
            return _db.Users.Where(user => user.Login ==  login && user.Password == password).Any();
        }
    }
}