using MediatR;
using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Application.ContentBlocks.Queries.GetContentBlock;

public sealed record GetContentBlockQuery(Guid Id) : IRequest<Result<ContentBlockResponse>>;
