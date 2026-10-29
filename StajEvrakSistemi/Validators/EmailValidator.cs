using System.Text.RegularExpressions; // Regex kullanabilmek için

namespace StajEvrakSistemi.Validators;

public class EmailValidator : BaseValidator
{
    private const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public override ValidationResult Validate(string input)
    {
        if(string.IsNullOrWhiteSpace(input) || !Regex.IsMatch(input.Trim(), pattern))
        {
            return ValidationResult.Failure("Geçerli bir e-posta formatı giriniz (örn: ad@alanadi.uzantı).");
            
        }
        return ValidationResult.Success();
    }
}