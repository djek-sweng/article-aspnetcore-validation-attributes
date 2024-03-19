namespace CustomValidationAttributes.WebApi.Controllers;

[ApiController]
[Route("api")]
public class TestController : ControllerBase
{
    public TestController()
    {
    }

    [HttpPost("test-letters-only")]
    public IActionResult TestLettersOnly([FromQuery] [LettersOnly] string text)
    {
        /* Place your business code here. */

        return Ok(text);
    }

    [HttpPost("test-of-legal-age")]
    public IActionResult TestOfLegalAge([FromQuery] [OfLegalAge] int value)
    {
        /* Place your business code here. */

        return Ok(value);
    }

    [HttpPost("test-user")]
    public IActionResult TestUser([FromBody] User user)
    {
        /* Place your business code here. */

        return Ok(user);
    }
}
