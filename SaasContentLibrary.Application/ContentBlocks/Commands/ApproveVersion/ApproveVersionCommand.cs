using MediatR;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.ApproveVersion
{
    public sealed record ApproveVersionCommand(
        Guid ContentBlockId,
        Guid VersionId,
        string ApprovedBy) : IRequest<Result>;
}
