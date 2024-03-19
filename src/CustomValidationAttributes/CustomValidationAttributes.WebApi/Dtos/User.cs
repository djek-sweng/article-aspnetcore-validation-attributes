namespace CustomValidationAttributes.WebApi.Dtos;

public class User
{
    [LettersOnly]
    public string Name { get; set; } = string.Empty;

    [OfLegalAge]
    public int Age { get; set; } = 0;
}
