### Custom validation attributes with ASP.NET Core - bouncer for your C# web-API
Incoming data validation is a standard requirement when implementing your [web-API](https://en.wikipedia.org/wiki/Web_API). Because before your data processing can take place, you have to ensure the integrity of the incoming data. [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) allows you to easily meet this requirement, because you can integrate your own validators with little effort.

#### **Advantages**
You have the following advantages:
* You create modular validators based on the [single-responsibility principle](https://en.wikipedia.org/wiki/Single-responsibility_principle).
* Your validators can be integrated using attributes at the data model level.
* Your business code focuses on functionality and is free of validation.
* The unit tests of your validators are greatly simplified.

#### **Getting started**
In the following, you will implement a validator for a specific application example. Your validator should ensure that a character string only consists of upper and lower case letters. The completed validator is then used to check a username. The username is transmitted to a web-API via a HTTP request.

#### **Step one - create class**
At the beginning create a new class including the associated file and then name both according to the following pattern {UseCase}Attribute. For this example, the name should be `LettersOnlyAttribute`.

```csharp
// File: LettersOnlyAttribute.cs

namespace CustomValidationAttributes.Validation.Attributes;

public class LettersOnlyAttribute
{
}
```

#### **Step two - add references**
Then add the following references at the beginning of the class file or add them into a dedicated global usings file.

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
```

#### **Step three - derive and decorate class**
Then derive from the base class `ValidationAttribute` and set the `sealed` modifier to prevent further derivations. Then decorate your class with the `AttributeUsage` attribute to define its usability. You can choose the following settings.

```csharp
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field |
  AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class LettersOnlyAttribute : ValidationAttribute
{
}
```

The choice of `AttributeTargets` defines the applicability of the validator to the desired elements in your code. With `AllowMultiple` you define whether more than one instance of the validator can be applied to one code element. The default value here is `false`.

#### **Step four - perform validation**
Now overwrite the `IsValid` method of the base class `ValidationAttribute`. First you convert to the target data type and then implement your validation logic.

```csharp
public override bool IsValid(object? value)
{
    if (value is null)
    {
        return false;
    }

    var text = (string)value;

    return IsLettersOnly(text);
}
```

The return value of the validation logic must be of the data type `bool`. Here you can, for example, use the `Regex` class to check regular expressions.

```csharp
using System.Text.RegularExpressions;

private static bool IsLettersOnly(string text)
{
    var regex = new Regex("^[a-zA-Z]*$");

    return regex.IsMatch(text);
}
```

#### **Step five - format the error message (optional)**
Optionally, you can also overwrite the `FormatErrorMessage` method. So you can make the error message a little more descriptive. The error message is generated automatically, if the validation fails.

```csharp
public override string FormatErrorMessage(string name)
{
    return string.Format(CultureInfo.CurrentCulture,
        $"The property, field or parameter '{name}' is invalid, " +
         "because only letters are allowed.");
}
```

#### **Unit test**
For the unit test it is now sufficient to test the `IsValid` method of the `LettersOnlyAttribute` class.

```csharp
using FluentAssertions;
using Xunit;
using CustomValidationAttributes.Validation.Attributes;

namespace CustomValidationAttributes.Validation.Test.Attributes;

public class LettersOnlyAttributeTest
{
    private readonly LettersOnlyAttribute _uut;

    public LettersOnlyAttributeTest()
    {
        _uut = new LettersOnlyAttribute();
    }

    [Theory]
    [InlineData("ValidInput", true)]
    [InlineData("Invalid_Input", false)]
    public void Test_IsValid(string input, bool result)
    {
        var actual = _uut.IsValid(input);

        actual.Should().Be(result);
    }
}
```

#### **Application example**
In the following application example you use the created validation attribute to secure the data transfer in your web-API. Your validator should validate the name of a user here. The username can only consist of upper and lower case letters.

#### **Variant A - in the data model (property)**
You place the validator directly in the `User` data model.

```csharp
public class User
{
    [LettersOnly]
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; } = 0;
}
```

The data is transferred to the web-API via the controller `TestController` and processed there.

```csharp
namespace CustomValidationAttributes.WebApi.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    public TestController()
    {
    }

    [HttpPost("api/test-user")]
    public IActionResult TestUser([FromBody] User user)
    {
        /* Place your business code here. */

        return Ok(user);
    }
}
```

The method named `TestUser` corresponds to a HTTP POST request with data transfer via the request body.

```sh
curl -X POST "https://localhost:5001/api/test-user" \
     -H "accept: */*" \
     -H "Content-Type: application/json" \
     -d "{\"name\":\"ArthurDent\",\"age\":42}"
```

#### **Variant B - on the controller method (parameters)**
You also have the option of integrating the validator directly on a parameter of your controller method.

```csharp
[HttpPost("api/test-letters-only")]
public IActionResult TestLettersOnly([FromQuery] [LettersOnly] string text)
{
    /* Place your business code here. */

    return Ok(text);
}
```

The method named `TestLettersOnly` corresponds to a HTTP POST request with data transfer via the request URL.

```sh
curl -X POST "https://localhost:5001/api/test-letters-only?text=ArthurDent" \
     -H "accept: */*" \
     -d ""
```

#### **Integration test**
Start the web-API application and execute the shown HTTP requests via your console. If the validator accepts your data entry, then your business code will be executed. In the example shown, the web-API simply sends back the input in the HTTP response body.

**Status code 200 (Ok) - Response body:**
```sh
{
  "name": "ArthurDent",
  "age": 42
}
```

However, if the validator rejects your data entry because you have violated the test criterion from the `IsLettersOnly` method, the controller will reject your HTTP request. The controller will then respond with the status code 400 (Bad Request) and will return the following response body. Your business code will not be executed.

**Status code 400 (Bad Request) - Response body:**
```sh
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-48f9cc895b0ce04e89a4e48cf1046e5e-f81c15186260824d-00",
  "errors": {
    "Name": [
      "The property, field or parameter 'Name' is invalid, because only letters are allowed. The value of 'Name' is 'ArthurDent_42', but must match regex pattern '^[a-zA-Z]*$'."
    ]
  }
}
```

#### **Conclusion**
The example discussed will hopefully show you how to implement and integrate your own validation attributes under ASP.NET Core. If you want to use validation attributes in the future, consider them early on when planning your web-API.

You can find the complete code in this GitHub repository.

For a quick start, you will find a web-API (including Swagger) in this GitHub repository, ready for your own tests. You can also take a look at the implementation of another validator named `OfLegalAgeAttribute`. This validator checks the `Age` in `User`, to ensure that the user is of legal age.

Happy Coding!
