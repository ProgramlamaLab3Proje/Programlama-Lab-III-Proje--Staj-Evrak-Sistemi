
using System.Globalization;
using System.Text.RegularExpressions;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

public class DocumentFieldComparison
{
    public string FieldName { get; set; } = "";
    public string DocumentValue { get; set; } = "";
    public string EnteredValue { get; set; } = "";
    public bool IsMatch { get; set; }
}

public static class DocumentComparisonService
{
    public static List<DocumentFieldComparison> Compare(
        ExtractedStudentFields extracted,
        StudentRecord entered)
    {
        return new List<DocumentFieldComparison>
        {
            CompareText(
                "T.C. Kimlik No",
                extracted.TcKimlikNo,
                entered.NationalId,
                NormalizeDigits),

            CompareText(
                "Öğrenci Numarası",
                extracted.StudentNumber,
                entered.StudentNumber,
                NormalizeDigits),

            CompareDate(
                extracted.BirthDate,
                entered.BirthDate),

            CompareText(
                "Telefon",
                extracted.Phone,
                entered.PhoneNumber,
                NormalizePhone),

            CompareText(
                "Adres",
                extracted.Address,
                entered.Address,
                NormalizeAddress)
        };
    }

    private static DocumentFieldComparison CompareText(
        string fieldName,
        string? documentValue,
        string? enteredValue,
        Func<string, string> normalize)
    {
        string document = documentValue?.Trim() ?? "";
        string form = enteredValue?.Trim() ?? "";

        return new DocumentFieldComparison
        {
            FieldName = fieldName,
            DocumentValue = document,
            EnteredValue = form,
            IsMatch = document.Length > 0 &&
                      form.Length > 0 &&
                      normalize(document) == normalize(form)
        };
    }

    private static DocumentFieldComparison CompareDate(
        string? documentValue,
        DateTime enteredValue)
    {
        bool documentDateValid = DateTime.TryParse(
            documentValue,
            CultureInfo.GetCultureInfo("tr-TR"),
            DateTimeStyles.None,
            out DateTime documentDate);

        bool enteredDateValid = enteredValue != DateTime.MinValue;

        return new DocumentFieldComparison
        {
            FieldName = "Doğum Tarihi",
            DocumentValue = documentValue ?? "",
            EnteredValue = enteredDateValid
                ? enteredValue.ToString("dd.MM.yyyy")
                : "",
            IsMatch = documentDateValid &&
                      enteredDateValid &&
                      documentDate.Date == enteredValue.Date
        };
    }

    private static string NormalizeDigits(string value)
    {
        return Regex.Replace(value, @"\D", "");
    }

    private static string NormalizePhone(string value)
    {
        string digits = NormalizeDigits(value);

        if (digits.StartsWith("90") && digits.Length == 12)
            digits = digits[2..];

        if (digits.StartsWith("0") && digits.Length == 11)
            digits = digits[1..];

        return digits;
    }

    private static string NormalizeAddress(string value)
    {
        return Regex.Replace(
            value.ToLowerInvariant(),
            @"[^\p{L}\p{N}]",
            "");
    }
}
