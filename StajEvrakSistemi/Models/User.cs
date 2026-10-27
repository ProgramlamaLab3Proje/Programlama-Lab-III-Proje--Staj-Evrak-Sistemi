namespace StajEvrakSistemi.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";

        public string Email { get; set; } = "";
        public string HashedPassword { get; set; } = "";

        public int FailedLogins { get; set; }
        public bool IsLocked { get; set; }

        public string Role { get; set; } = "";
    }
}
