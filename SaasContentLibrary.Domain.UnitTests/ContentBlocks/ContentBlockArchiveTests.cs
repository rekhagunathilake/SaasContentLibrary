using FluentAssertions;
using SaasContentLibrary.Domain.ContentBlocks;
using SaasContentLibrary.Domain.ContentBlocks.Events;
using SaasContentLibrary.Domain.UnitTests.TestHelpers;

namespace SaasContentLibrary.Domain.UnitTests.ContentBlocks;

public class ContentBlockArchiveTests
{
    [Fact]
    public void Archive_OnActiveBlock_SetArchivedStatusAndTimeStamp()
    {
        var block = ContentBlockFactory.CreateValid();
        var archivedTime = ContentBlockFactory.FixedNotUtc.AddDays(30);

        var result = block.Archive(archivedTime);

        result.IsSuccess.Should().BeTrue();
        block.Status.Should().Be(BlockStatus.Archived);
        block.ArchivedAtUtc.Should().Be(archivedTime);
        block.DomainEvents.OfType<ContentBlockArchivedEvent>().Should().ContainSingle();
    }

    [Fact]
    public void Archive_OnAlreadyArchivedBlock_IsIdempotent()
    {
        var block = ContentBlockFactory.CreateValid();
        var originalArchivedTime = ContentBlockFactory.FixedNotUtc.AddDays(30);
        block.Archive(originalArchivedTime);
        block.ClearDomainEvents();

        var result = block.Archive(ContentBlockFactory.FixedNotUtc.AddDays(60));
        
        result.IsSuccess.Should().BeTrue();
        block.ArchivedAtUtc.Should().Be(originalArchivedTime); // no change
        block.DomainEvents.Should().BeEmpty(); // no second event
    }
}
