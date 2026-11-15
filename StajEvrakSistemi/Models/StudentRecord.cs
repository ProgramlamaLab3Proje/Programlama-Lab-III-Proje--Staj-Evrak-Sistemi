namespace StajEvrakSistemi.Models
{
    public class StudentRecord : StudentInformations
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? DocumentPath { get; set; }
        public BelgeDurumu Status { get; set; } = BelgeDurumu.Yuklendi;
    }
}
