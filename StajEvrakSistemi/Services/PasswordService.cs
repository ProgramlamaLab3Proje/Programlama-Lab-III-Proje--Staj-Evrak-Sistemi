namespace StajEvrakSistemi.Services
{
    public class PasswordService
    {
        public string HashPassword(string sifre)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(sifre);
            return passwordHash;
        }
        public bool VerifyPassword(string sifre, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(sifre, passwordHash);
        }
    }
}

