namespace SaasContentLibrary.Domain
{
    public record ContentBlock
    {
        ContentBlockId Id { get; set; }
        Guid TenantId { get; set; }
        BlockType BlockType { get; set; }
        string Locale { get; set; }
        string Name { get; set; }
        int Status { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime ArchivedAt { get; set; }
        ContentVersion ContentVersion { get; set; }
    }
}
