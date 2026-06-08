using SaasContentLibrary.Domain.ContentBlocks;

namespace SaasContentLibrary.Api.RequestDTOs;

public sealed record ListContentBlocksRequest(Guid TenantId, BlockType? BlockType);
