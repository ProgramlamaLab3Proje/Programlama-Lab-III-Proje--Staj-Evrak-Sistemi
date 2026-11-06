using StajEvrakSistemi.Models;
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Utility;
using StajEvrakSistemi.Services;

namespace StajEvrakSistemi.Data
{
    public class AdminSeeder
    {
        private readonly Context _context;
        private readonly PasswordService _passwordService;
        public AdminSeeder(Context context, PasswordService passwordService) // veritabanına ve şifre hashlemeye erişim sağlanır bu constructor yapısı sayesinde
        {
            _context = context;
            _passwordService = passwordService;
        }
        public void Seed()
        {
            bool adminControl = _context.Admins.Any();

            if (!adminControl)
            {
                var admin = new Admin
                {
                    Name = "System",
                    Surname = "Admin",
                    Email = "admin@stajevraksistemi.com",
                    HashedPassword = _passwordService.HashPassword("Admin123!"),
                    Role = Utility.StaticRoles.ROLE_ADMIN,
                    FailedLogins = 0,
                    IsLocked = false
                };
            }
        }
    }

}
