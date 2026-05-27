namespace SaasContentLibrary.Domain.ContentBlocks
{
    public readonly record struct ContentBlockId(Guid Value)
    {
        public static ContentBlockId NewId() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}