namespace StajEvrakSistemi.Validators;

public class TcValidator : BaseValidator
{
    public bool IsForeign { get; set; }
    public TcValidator(bool isForeign = false)
    {
        IsForeign = isForeign;
    }
    public override ValidationResult Validate(string input)
    {
        // 11 hane ve rakam kontrolü
        if(string.IsNullOrWhiteSpace(input) || input.Length != 11 || !input.All(char.IsDigit))
        {
            return ValidationResult.Failure("TC Kimlik Numarası 11 haneli ve sadece rakamlardan oluşmalıdır.");
            
        }
        // yabancı uyruklu öğrenci kontrolü
        if(IsForeign)
        {
            if(!input.StartsWith("99"))
            {
                return ValidationResult.Failure("Yabancı uyruklu öğrenci kimlik numarası 99 ile başlamalıdır.");
                
            }
            return ValidationResult.Success();
        }
        // TC vatandaşı ise ilk hane 0 olamaz kontrolü
        if(input.StartsWith("0"))
        {
            return ValidationResult.Failure("TC Kimlik Numarası 0 ile başlayamaz.");
            
        }
        return ValidationResult.Success();
    }
}