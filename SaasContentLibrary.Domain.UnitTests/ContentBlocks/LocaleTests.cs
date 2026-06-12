using FluentAssertions;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Domain.UnitTests.ContentBlocks;

public class LocaleTests
{
    [Theory]
    [InlineData("en")]
    [InlineData("en-US")]
    [InlineData("en-Hans")]
    [InlineData("en-Hans-CN")]
    public void Create_WithValidBcp47_ReturnsSuccess(string code)
    {
        var result = Locale.Create(code);

        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithBlankCode_Fails(string blank)
    {
        var result = Locale.Create(blank);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Locale.Empty");
    }

    [Theory]
    [InlineData("EN")]
    [InlineData("en_US")]
    [InlineData("english")]
    [InlineData("123")]
    public void Create_WithInvalidCode_Fails(string invalidCode)
    {
        var result = Locale.Create(invalidCode);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Locale.Invalid");
    }
}
