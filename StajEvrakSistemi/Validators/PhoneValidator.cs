using System.Text.RegularExpressions; // metin formatı doğrulamak için Regex(Regular Expression) ekledim.

namespace StajEvrakSistemi.Validators;

public class PhoneValidator : BaseValidator
{
    private const string pattern = @"^\+90\s?[0-9]{3}\s?[0-9]{3}\s?[0-9]{2}\s?[0-9]{2}$";
    public override bool Validate(string input, out string errorMessage)
    {
        errorMessage = string.Empty;
        
        if(string.IsNullOrWhiteSpace(input) || !Regex.IsMatch(input.Trim(), pattern))
        {
            errorMessage = "Telefon numarası +90 xxx xxx xx xx formatında olmalıdır.";
            return false;
        }
        return true;
    }
}