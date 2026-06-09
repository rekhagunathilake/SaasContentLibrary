using SaasContentLibrary.Application.ContentBlocks.Commands.AddDraftVersion;

namespace SaasContentLibrary.Api.RequestDTOs;

public sealed record AddDraftVersionRequest(string contentBody, string authoredBy)
{
    public AddDraftVersionCommand ToCommand(Guid contentBlockId) => new(contentBlockId, contentBody, authoredBy);
}
