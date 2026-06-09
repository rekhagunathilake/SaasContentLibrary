using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.Common;

public static class ApplicationErrors
{
    public static Error ContentBlockNotFound(Guid contentBlockId) => 
        Error.NotFound(
            code: "ContentBlock.NotFound",
            message: $"Content block with ID '{contentBlockId}' was not found.");
}
