namespace CustomValidationAttributes.Validation.Test.Attributes;

public class OfLegalAgeAttributeTest
{
    private readonly OfLegalAgeAttribute _uut;

    public OfLegalAgeAttributeTest()
    {
        _uut = new OfLegalAgeAttribute();
    }

    [Theory]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    [InlineData(100)]
    public void Should_BeValid(int input)
    {
        var result = _uut.IsValid(input);

        result.Should().Be(true);
    }

    [Theory]
    [InlineData(17)]
    [InlineData(16)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_BeInvalid(int input)
    {
        var result = _uut.IsValid(input);

        result.Should().Be(false);
    }
}
