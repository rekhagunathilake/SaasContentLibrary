using FluentAssertions;
using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks;
using SaasContentLibrary.Domain.ContentBlocks.Events;
using SaasContentLibrary.Domain.UnitTests.TestHelpers;

namespace SaasContentLibrary.Domain.UnitTests.ContentBlocks;

public class ContentBlockTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsActiveBlockWithVersionOneInDraft() 
    {
        var block = ContentBlockFactory.CreateValid();

        block.Status.Should().Be(BlockStatus.Active);
        block.Versions.Should().HaveCount(1);
        block.Versions.First().VersionNumber.Should().Be(1);
        block.Versions.First().VersionStatus.Should().Be(VersionStatus.Draft);
        block.CreatedAtUtc.Should().Be(ContentBlockFactory.FixedNowUtc);
        block.ArchivedAtUtc.Should().BeNull();
    }

    [Fact]
    public void Create_RaisesContentBlockCreatedAndVersionAddedEvents()
    {
        var block = ContentBlockFactory.CreateValid();

        block.DomainEvents.Should().HaveCount(2);
        block.DomainEvents.OfType<ContentBlockCreatedEvent>().Should().ContainSingle();
        block.DomainEvents.OfType<VersionAddedEvent>().Should().ContainSingle();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Fails(string blankName) 
    {
        var result = ContentBlock.Create(
            tenantId: TenantId.NewId(),
            blockType: BlockType.Disclaimer,
            locale: Locale.Create("en-US").Value,
            name: blankName,
            initialBody: ContentBlockFactory.TestBody(),
            authoredBy: "test@test.com",
            nowUtc: ContentBlockFactory.FixedNowUtc
            );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContentBlockErrors.NameEmpty);
    }

    [Fact]
    public void Create_WithNameExceedingMaxLength_Fails()
    {
        var tooLong = new string('a', ContentBlock.NameMaxLength + 1);

        var result = ContentBlock.Create(
            tenantId: TenantId.NewId(),
            blockType: BlockType.Disclaimer,
            locale: Locale.Create("en-US").Value,
            name: tooLong,
            initialBody: ContentBlockFactory.TestBody(),
            authoredBy: "test@test.com",
            nowUtc: ContentBlockFactory.FixedNowUtc);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ContentBlock.NameTooLong");
    }

    [Fact]
    public void Create_WithBlankAuthoredBy_Fails()
    {
        var result = ContentBlock.Create(
            tenantId: TenantId.NewId(),
            blockType: BlockType.Disclaimer,
            locale: Locale.Create("en-US").Value,
            name: "Test name",
            initialBody: ContentBlockFactory.TestBody(),
            authoredBy: "   ",
            nowUtc: ContentBlockFactory.FixedNowUtc);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContentBlockErrors.AuthorEmpty);
    }

    [Fact]
    public void AddDraftVersion_OnActiveBlock_AppendsMonotonicVersion()
    {
        var block = ContentBlockFactory.CreateValid();

        var result = block.AddDraftVersion(
            ContentBlockFactory.TestBody("Revised Disclaimer Text"),
            authoredBy: "rekha@test.com",
            nowUtc: ContentBlockFactory.FixedNowUtc.AddDays(1));

        result.IsSuccess.Should().BeTrue();
        block.Versions.Should().HaveCount(2);
        block.Versions.Last().VersionNumber.Should().Be(2);
        block.Versions.Last().VersionStatus.Should().Be(VersionStatus.Draft);
    }

    [Fact]
    public void AddDraftVersion_OnArchivedBlock_Fails()
    {
        var block = ContentBlockFactory.CreateValid();
        block.Archive(ContentBlockFactory.FixedNowUtc.AddHours(1));

        var result = block.AddDraftVersion(
            ContentBlockFactory.TestBody(),
            authoredBy: "rekha@test.com",
            nowUtc: ContentBlockFactory.FixedNowUtc.AddHours(2));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContentBlockErrors.IsArchived);
    }

    [Fact]
    public void SubmitForReview_OnDraftVersion_TransitionsToInReview()
    {
        var block = ContentBlockFactory.CreateValid();
        var versionId = block.Versions.First().Id;

        var result = block.SubmitForReview(versionId, ContentBlockFactory.FixedNowUtc.AddHours(1));

        result.IsSuccess.Should().BeTrue();
        block.Versions.First().VersionStatus.Should().Be(VersionStatus.InReview);
        block.DomainEvents.OfType<VersionSubmittedForReviewEvent>().Should().ContainSingle();
    }

    [Fact]
    public void SubmitForReview_OnNonExistingVersion_Fails()
    {
        var block = ContentBlockFactory.CreateValid();

        var result = block.SubmitForReview(ContentVersionId.NewId(), ContentBlockFactory.FixedNowUtc);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContentBlockErrors.VersionNotFound);
    }

    [Fact]
    public void SubmitForReview_OnAlreadySubmittedVersion_Fails()
    {
        var block = ContentBlockFactory.CreateValid();
        var versionId = block.Versions.First().Id;
        block.SubmitForReview(versionId, ContentBlockFactory.FixedNowUtc.AddHours(1));

        var result = block.SubmitForReview(versionId, ContentBlockFactory.FixedNowUtc.AddHours(2));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContentBlockErrors.VersionNotDraft);
    }
}
