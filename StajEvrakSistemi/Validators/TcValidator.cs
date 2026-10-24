namespace StajEvrakSistemi.Validators;

public class TcValidator : BaseValidator
{
    public bool IsForeign { get; set; }
    public TcValidator(bool isForeign = false)
    {
        IsForeign = isForeign;
    }
    public override bool Validate(string input, out string errorMessage)
    {
        errorMessage = string.Empty;

        // 11 hane ve rakam kontrolü
        if(string.IsNullOrWhiteSpace(input) || input.Length != 11 || !long.TryParse(input, out _))
        {
            errorMessage = "TC Kimlik Numarası 11 haneli ve sadece rakamlardan oluşmalıdır.";
            return false;
        }
        // yabancı uyruklu öğrenci kontrolü
        if(IsForeign)
        {
            if(!input.StartsWith("99"))
            {
                errorMessage = "Yabancı uyruklu öğrenci kimlik numarası 99 ile başlamalıdır.";
                return false;
            }
            return true;
        }
        // TC vatandaşı ise ilk hane 0 olamaz kontrolü
        if(input.StartsWith("0"))
        {
            errorMessage = "TC Kimlik Numarası 0 ile başlayamaz.";
            return false;
        }
        return true;
    }
}