using System.Text.RegularExpressions;

namespace StajEvrakSistemi.Services;

// Belgeden çıkarılan öğrenci bilgilerini tutar.
public class ExtractedStudentFields
{
    public string? TcKimlikNo { get; set; }
    public string? FullName { get; set; }
    public string? StudentNumber { get; set; }
    public string? BirthDate { get; set; }
    public string? AcademicYear { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public static class DocumentFieldExtractor
{
    public static ExtractedStudentFields ExtractStudentFields(string text)
    {
        var fields = new ExtractedStudentFields();

        string tcSection = ExtractBetweenLabels(
            text,
            @"T\.C\.\s*KİMLİK\s*NO\s*/\s*UYRUĞU",
            @"BÖLÜMÜ"
        ) ?? string.Empty;

        var tcMatch = Regex.Match(tcSection, @"\b\d{11}\b");
        fields.TcKimlikNo = tcMatch.Success ? tcMatch.Value : null;

        fields.FullName = ExtractBetweenLabels(
            text,
            @"ADI\s*SOYADI",
            @"DAHA\s*ÖNCE\s*YAPTIĞI\s*STAJ\s*GÜN\s*SAYISI"
        );

        fields.BirthDate = ExtractBetweenLabels(
            text,
            @"DOĞUM\s*TARİHİ",
            @"ÖĞRETİM\s*YILI"
        );

        fields.AcademicYear = ExtractBetweenLabels(
            text,
            @"ÖĞRETİM\s*YILI",
            @"ÖĞRENCİ\s*NUMARASI"
        );

        fields.StudentNumber = ExtractBetweenLabels(
            text,
            @"ÖĞRENCİ\s*NUMARASI",
            @"E-POSTA\s*ADRESİ"
        );

        string contactSection = ExtractBetweenLabels(
            text,
            @"E-POSTA\s*ADRESİ\s*/?\s*TELEFON\s*NO",
            @"İKAMETGÂH\s*ADRESİ"
        ) ?? string.Empty;

        var emailMatch = Regex.Match(
            contactSection,
            @"[\w.%+-]+@[\w.-]+"
        );

        fields.Email = emailMatch.Success ? emailMatch.Value : null;

        var phoneMatch = Regex.Match(
            contactSection,
            @"\+?\d[\d\s().-]*\d"
        );

        fields.Phone = phoneMatch.Success
            ? phoneMatch.Value.Trim()
            : null;

        fields.Address = ExtractBetweenLabels(
            text,
            @"İKAMETGÂH\s*ADRESİ",
            @"SAĞLIK\s*YARDIM\s*STATÜSÜ"
        );

        return fields;
    }

    // İki alan başlığı arasındaki metni bulur.
    private static string? ExtractBetweenLabels(
        string text,
        string startLabel,
        string endLabel)
    {
        var match = Regex.Match(
            text,
            startLabel + @"\s*(.*?)\s*(?=" + endLabel + @")",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        return match.Success
            ? match.Groups[1].Value.Trim()
            : null;
    }
}
