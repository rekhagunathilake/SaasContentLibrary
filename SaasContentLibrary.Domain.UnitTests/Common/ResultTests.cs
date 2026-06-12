using FluentAssertions;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ExposesValueWithoutRecursion()
    {
        // Specifically guards against the Result<T>.Value -> Value recursion bug (Commit: df904535552cbe6bf2318fd24b115cd42c27c971)
        // which caused a stack overflow before being fixed.
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();

        result.Value.Should().Be(42);
        result.Value.Should().Be(42); // calling twice to be sure it's idempotent
    }

    [Fact]
    public void Failure_AccessingValue_ThrowsInvalidOperationException()
    {
        var result = Result.Failure<int>(Error.Validation("Test.Error", "Test Message"));

        Action act = () => _ = result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_ExposesErrorCorrectly()
    {
        var error = Error.Validation("Test.Code", "Test Message");

        var result = Result.Failure<string>(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
