namespace StajEvrakSistemi.Validators;

public abstract class BaseValidator
{
    public abstract bool Validate(string input, out string errorMessage);
}