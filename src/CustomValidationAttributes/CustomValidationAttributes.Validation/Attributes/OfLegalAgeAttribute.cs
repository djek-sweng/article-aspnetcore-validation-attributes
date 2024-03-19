namespace CustomValidationAttributes.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public sealed class OfLegalAgeAttribute : ValidationAttribute
{
    private const int LegalAge = 18;
    private int _age = -1;

    public OfLegalAgeAttribute()
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return false;
        }

        _age = (int)value;

        return IsOfLegalAge(_age);
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(CultureInfo.CurrentCulture,
            $"The property, field or parameter '{name}' is invalid, because no legal age is given. " +
            $"The value of '{name}' is '{_age}', but must be at least '{LegalAge}'.");
    }

    private static bool IsOfLegalAge(int age)
    {
        return age > LegalAge - 1;
    }
}
