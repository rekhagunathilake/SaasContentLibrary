using MediatR;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.CreateContentBlock;

public sealed record CreateContentBlockCommand(
    Guid TenantId,
    BlockType BlockType,
    string LocaleCode,
    string Name,
    string ContentBody,
    string AuthoredBy) : IRequest<Result<Guid>>;