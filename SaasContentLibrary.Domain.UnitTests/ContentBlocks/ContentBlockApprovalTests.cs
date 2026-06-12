using FluentAssertions;
using SaasContentLibrary.Domain.ContentBlocks;
using SaasContentLibrary.Domain.ContentBlocks.Events;
using SaasContentLibrary.Domain.UnitTests.TestHelpers;

namespace SaasContentLibrary.Domain.UnitTests.ContentBlocks;

public class ContentBlockApprovalTests
{
	[Fact]
	public void ApproveVersion_OnInReviewVersion_TransitionsToApproved()
	{
		var block = ContentBlockFactory.CreateValid();
		var versionId = block.Versions.First().Id;
		block.SubmitForReview(versionId, ContentBlockFactory.FixedNotUtc.AddHours(1));

		var result = block.ApproveVersion(versionId,
			approvedBy: "rekha@test.com",
			nowUtc: ContentBlockFactory.FixedNotUtc.AddHours(2));

		result.IsSuccess.Should().BeTrue();
		block.Versions.First().VersionStatus.Should().Be(VersionStatus.Approved);
		block.Versions.First().ApprovedBy.Should().Be("rekha@test.com");
		block.Versions.First().ApprovedOnUtc.Should().Be(ContentBlockFactory.FixedNotUtc.AddHours(2));
	}

	[Fact]
	public void ApproveVersion_RaisesVersionApprovedEventWithNoSupersededId_WhenNoPriorApproved()
	{
        var block = ContentBlockFactory.CreateValid();
        var versionId = block.Versions.First().Id;
        block.SubmitForReview(versionId, ContentBlockFactory.FixedNotUtc.AddHours(1));

		block.ApproveVersion(versionId, "rekha@test.com", ContentBlockFactory.FixedNotUtc.AddHours(2));

		var approvedEvent = block.DomainEvents.OfType<VersionApprovedEvent>().Single();
		approvedEvent.ApprovedVersionId.Should().Be(versionId);
		approvedEvent.SupersededVersionId.Should().BeNull();
    }

	[Fact]
	public void ApproveVersion_AutomaticallyDemotesPriorApprovedToSuperseded()
	{
        // Set a block with version v1 already approved
		var block = ContentBlockFactory.CreateValid();
		var v1Id = block.Versions.First().Id;
		block.SubmitForReview(v1Id, ContentBlockFactory.FixedNotUtc.AddHours(1));
		block.ApproveVersion(v1Id, "rekha@test.com", ContentBlockFactory.FixedNotUtc.AddHours(2));

		// Add v2, submit, approve
		block.AddDraftVersion(ContentBlockFactory.TestBody("Revised!"),
			"rekha@revised.com",
			ContentBlockFactory.FixedNotUtc.AddDays(1));

		var v2Id = block.Versions.Last().Id;
		block.SubmitForReview(v2Id, ContentBlockFactory.FixedNotUtc.AddDays(1).AddHours(1));

		block.ClearDomainEvents(); // discard pre-approval events for cleaner assertion

		// Act
		var result = block.ApproveVersion(v2Id, "rekha@test.com", ContentBlockFactory.FixedNotUtc.AddDays(1).AddHours(2));

		// Assert
		result.IsSuccess.Should().BeTrue();
		block.Versions.First().VersionStatus.Should().Be(VersionStatus.Superseded);
		block.Versions.Last().VersionStatus.Should().Be(VersionStatus.Approved);

		// Exactly one Approved version at any time. - the core invariant!
		block.Versions.Count(v => v.VersionStatus == VersionStatus.Approved).Should().Be(1);

		// Event carries the superseded Id
		var approvedEvent = block.DomainEvents.OfType<VersionApprovedEvent>().Single();
		approvedEvent.ApprovedVersionId.Should().Be(v2Id);
		approvedEvent.SupersededVersionId.Should().Be(v1Id);
    }

	[Fact]
	public void ApproveVersion_OnDraftVersion_Fails()
	{
        var block = ContentBlockFactory.CreateValid();
        var versionId = block.Versions.First().Id;

        var result = block.ApproveVersion(versionId, "rekha@test.com", ContentBlockFactory.FixedNotUtc.AddHours(1));

		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(ContentBlockErrors.VersionNotInReview);
    }

	[Fact]
	public void ApproveVersion_WithBlankApprover_Fails()
	{
        var block = ContentBlockFactory.CreateValid();
        var versionId = block.Versions.First().Id;
        block.SubmitForReview(versionId, ContentBlockFactory.FixedNotUtc.AddHours(1));

		var result = block.ApproveVersion(
			versionId, approvedBy: " ", nowUtc: ContentBlockFactory.FixedNotUtc.AddHours(2));

		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(ContentBlockErrors.ApproverEmpty);
    }
}
