namespace StajEvrakSistemi.Validators;

public abstract class BaseValidator
{
    public abstract ValidationResult Validate(string input);
}