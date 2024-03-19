namespace CustomValidationAttributes.Validation.Test.Attributes;

public class LettersOnlyAttributeTest
{
    private readonly LettersOnlyAttribute _uut;

    public LettersOnlyAttributeTest()
    {
        _uut = new LettersOnlyAttribute();
    }

    [Theory]
    [InlineData("ValidInput")]
    [InlineData("validinput")]
    [InlineData("VALIDINPUT")]
    public void Should_BeValid(string input)
    {
        var result = _uut.IsValid(input);

        result.Should().Be(true);
    }

    [Theory]
    [InlineData("Invalid Input")]
    [InlineData("invalid_input")]
    [InlineData("invalidinput0")]
    public void Should_BeInvalid(string input)
    {
        var result = _uut.IsValid(input);

        result.Should().Be(false);
    }
}
