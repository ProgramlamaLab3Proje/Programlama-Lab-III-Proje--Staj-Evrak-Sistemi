namespace StajEvrakSistemi.Models;

public class Holiday
{
    // Tatilin tarihi
    public DateTime Date { get; set; }

    // Tatilin adı
    public string Name { get; set; } = string.Empty;

    // Arife günleri gibi yarım günler için 
    public bool IsHalfDay { get; set; }
}