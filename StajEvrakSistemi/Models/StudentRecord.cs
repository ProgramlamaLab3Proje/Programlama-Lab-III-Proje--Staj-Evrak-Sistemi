namespace StajEvrakSistemi.Models
{
    public class StudentRecord : StudentInformations
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
