namespace StajEvrakSistemi.Models
{
    public class Student : User
    {
        public string StudentNumber { get; set; } = "";
        public string Department { get; set; } = "";
        public string Nationality { get; set; } = "";
        public string NationalId { get; set; } = "";
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public int PreviousInternshipDays { get; set; }
        public string? FaxNumber { get; set; }
        public string HealthAssistanceStatus { get; set; } = "";
        public int AcademicYearStart { get; set; }
    }
}
