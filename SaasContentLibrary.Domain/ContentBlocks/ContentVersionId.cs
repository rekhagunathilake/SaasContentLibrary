namespace SaasContentLibrary.Domain.ContentBlocks
{
    public readonly record struct ContentVersionId(Guid Value)
    {
        public static ContentVersionId NewId() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
