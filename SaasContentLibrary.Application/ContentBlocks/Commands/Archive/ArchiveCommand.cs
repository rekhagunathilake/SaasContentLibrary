using MediatR;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Commands.Archive;

public sealed record ArchiveCommand(
    Guid ContentBlockId) : IRequest<Result>;
