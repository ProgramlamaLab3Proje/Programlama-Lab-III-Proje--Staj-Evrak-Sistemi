namespace StajEvrakSistemi.Models
{
    public class Student : User
    {
        public List<StudentRecord> Records { get; set; } = new();
    }
}
