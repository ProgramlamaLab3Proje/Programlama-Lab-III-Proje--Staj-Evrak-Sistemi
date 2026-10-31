namespace StajEvrakSistemi.Validators;

public class AddressValidator : BaseValidator
{
    public override ValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return ValidationResult.Failure("Adres boş bırakılamaz.");
        }

        string[] parts = input.Split(',') // adresi virgüllere göre parçalara ayrıması için
                              .Select(part => part.Trim())
                              .ToArray(); 

        if (parts.Length !=6) // tam olarak 6 bölüm olmalı
        {
            return ValidationResult.Failure(
                "Adres; Mahalle, Cadde/Sokak, Bina No, Daire No, İlçe ve İl sırasıyla yazılmalıdır.");
        }

        // ----- BOŞ BÖLÜM KONTROLÜ -----
        if (parts.Any(string.IsNullOrWhiteSpace))
        {
            return ValidationResult.Failure("Adresin hiçbir bölümü boş bırakılamaz.");
        }

        // ----- MAHALLE KONTROLÜ -----
        if (!parts[0].EndsWith("Mahallesi", StringComparison.OrdinalIgnoreCase) &&
            !parts[0].EndsWith("Mah.", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Failure(
                "Adresin ilk bölümü mahalle adı olmalıdır (örn: Yenimahalle Mahallesi).");
        }

        // ----- CADDE/SOKAK KONTROLÜ -----
        if (!parts[1].EndsWith("Caddesi", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].EndsWith("Cad.", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].EndsWith("Sokağı", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].EndsWith("Sokak", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].EndsWith("Sok.", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Failure(
                "Adresin ikinci bölümü cadde veya sokak adı olmalıdır.");
        }
        // ----- BİNA NO KONTROLÜ -----
        if(!parts[2].StartsWith("No:", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Failure(
                "Adresin üçüncü bölümü bina numarası olmalıdır (örn: NO: 15).");
        }

        string binaNo = parts[2].Substring(3).Trim();

        if (string.IsNullOrWhiteSpace(binaNo) ||
            !binaNo.Any(char.IsDigit))
        {
            return ValidationResult.Failure(
                "Bina numarası geçerli bir numara içermelidir.");
        }

        // ----- DAİRE NO KONTROLÜ -----
        if (!parts[3].StartsWith("Daire:", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Failure(
                "Adresin dördüncü bölümü daire numarası olmalıdır (örn: Daire: 4).");
        }
        string daireNo = parts[3].Substring(6).Trim();

        if (string.IsNullOrWhiteSpace(daireNo) ||
            !daireNo.Any(char.IsDigit))
        {
            return ValidationResult.Failure(
                "Daire numarası geçerli bir numara içermelidir.");
        }

        // İL VE İLÇE KONTROLÜ
        if (string.IsNullOrWhiteSpace(parts[4]) ||
            string.IsNullOrWhiteSpace(parts[5]))
        {
            return ValidationResult.Failure(
                "İlçe ve İl bilgileri boş bırakılamaz.");
        }
        return ValidationResult.Success();
    }
}