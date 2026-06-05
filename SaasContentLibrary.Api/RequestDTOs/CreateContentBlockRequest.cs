using SaasContentLibrary.Application.ContentBlocks.Commands.CreateContentBlock;
using SaasContentLibrary.Domain.ContentBlocks;

public sealed record CreateContentBlockRequest(
    Guid TenantId,
    BlockType BlockType,
    string LocaleCode,
    string Name,
    string Body,
    string AuthoredBy)
{
    public CreateContentBlockCommand ToCommand() =>
        new(TenantId, BlockType, LocaleCode, Name, Body, AuthoredBy);
}