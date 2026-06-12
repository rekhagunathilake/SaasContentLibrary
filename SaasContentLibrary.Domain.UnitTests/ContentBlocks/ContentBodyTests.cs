using FluentAssertions;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Domain.UnitTests.ContentBlocks;

public class ContentBodyTests
{
    [Fact]
    public void Create_WithValidText_ReturnsSuccess()
    {
        var result = ContentBody.Create("Valid Content Text");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Valid Content Text");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithBlankBody_Fails(string blank)
    {
        var result = ContentBody.Create(blank);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContentBlockErrors.BodyEmpty);
    }

    [Fact]
    public void Create_WithTextExceedingMaxLength_Fails()
    {
        var tooLong = new string('a', ContentBody.MaxLength + 1);

        var result = ContentBody.Create(tooLong);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ContentBody.TooLong");
    }
}
