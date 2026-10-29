using System.Text.RegularExpressions; // metin formatı doğrulamak için Regex(Regular Expression) ekledim.

namespace StajEvrakSistemi.Validators;

public class PhoneValidator : BaseValidator
{
    private const string Pattern = @"^\+90\s?[0-9]{3}\s?[0-9]{3}\s?[0-9]{2}\s?[0-9]{2}$";
    public override ValidationResult Validate(string input)
    {
        if(string.IsNullOrWhiteSpace(input) || !Regex.IsMatch(input.Trim(), Pattern))
        {
            return ValidationResult.Failure("Telefon numarası +90 xxx xxx xx xx formatında olmalıdır.");
           
        }
        return ValidationResult.Success();
    }
}