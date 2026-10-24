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
        if(string.IsNullOrWhiteSpace(input) || input.Length != 11 || !long.TryParse(input, out _))
        {
            errorMessage = "TC Kimlik Numarası 11 haneli ve sadece rakamlardan oluşmalıdır.";
            return false;
        }
    }
}