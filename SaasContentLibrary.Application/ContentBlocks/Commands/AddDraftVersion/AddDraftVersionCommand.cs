using MediatR;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.AddDraftVersion;

public sealed record AddDraftVersionCommand(
    Guid ContentBlockId,
    string ContentBody,
    string AuthoredBy) : IRequest<Result<Guid>>;
