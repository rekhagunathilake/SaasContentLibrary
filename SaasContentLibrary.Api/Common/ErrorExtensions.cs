using Microsoft.AspNetCore.Http.HttpResults;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Api.Common;

public static class ErrorExtensions
{
    public static ProblemHttpResult ToProblemDetails(this Error error)
    {
        var (statusCode, title) = MapStatus(error.code);

        return TypedResults.Problem(
            title: title,
            detail: error.message,
            statusCode: statusCode,
            type: $"https://content-library.local/errors/{error.code}",
            extensions: new Dictionary<string, object?>
            {
                ["code"] = error.code
            });
    }

    private static (int statusCode, string title) MapStatus(string errorCode) => errorCode switch
    {
        var c when c.EndsWith("NotFound", StringComparison.Ordinal) =>
        (StatusCodes.Status404NotFound, "Resource Not Found"),

        var c when c.EndsWith(".IsArchived", StringComparison.Ordinal)
        || c.EndsWith(".NotDraft", StringComparison.Ordinal)
        || c.EndsWith(".NotInReview", StringComparison.Ordinal) =>
        (StatusCodes.Status409Conflict, "Conflict with Current State"),

        _ => (StatusCodes.Status400BadRequest, "Validation failed")
    };
}
