using SaasContentLibrary.Domain.Common;
using SaasContentLibrary.Domain.ContentBlocks.Events;

namespace SaasContentLibrary.Domain.ContentBlocks
{
    public sealed class ContentBlock : AggregateRoot<ContentBlockId>
    {
        public const int NameMaxLength = 200;

        private readonly List<ContentVersion> _versions = new();

        public TenantId TenantId { get; private set; }
        public BlockType BlockType { get; private set; }
        public Locale Locale { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public BlockStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime ArchivedAtUtc { get; private set; }

        public IReadOnlyCollection<ContentVersion> Versions => _versions.AsReadOnly();

        private ContentBlock() { } // For EF Core

        private ContentBlock(
            ContentBlockId id,
            TenantId tenantId,
            BlockType blockType,
            Locale locale,
            string name,
            DateTime createdAtUtc) : base(id)
        {
            TenantId = tenantId;
            BlockType = blockType;
            Locale = locale;
            Name = name;
            Status = BlockStatus.Active;
            CreatedAtUtc = createdAtUtc;
        }

        public static Result<ContentBlock> Create(
            TenantId tenantId,
            BlockType blockType,
            Locale locale,
            string name,
            ContentBody initialBody,
            string authoredBy,
            DateTime nowUtc)
        {
            var nameValidation = ValidateName(name);
            if (nameValidation.IsFailure)
                return Result.Failure<ContentBlock>(nameValidation.Error);

            if (string.IsNullOrEmpty(authoredBy))
                return Result.Failure<ContentBlock>(ContentBlockErrors.AuthorEmpty);

            var block = new ContentBlock(
                ContentBlockId.NewId(), tenantId, blockType, locale,
                name.Trim(), nowUtc);

            var firstVersion = new ContentVersion(
                ContentVersionId.NewId(), versionNumber: 1,
                initialBody, authoredBy.Trim(), nowUtc);

            block._versions.Add(firstVersion);

            block.RaiseDomainEvents(new ContentBlockCreatedEvent(
                block.Id, tenantId, blockType, nowUtc));

            block.RaiseDomainEvents(new VersionAddedEvent(
                block.Id, firstVersion.Id, firstVersion.VersionNumber, nowUtc));

            return Result.Success(block);
        }

        public Result<ContentVersionId> AddDraftVersion(
            ContentBody body,
            string authoredBy,
            DateTime nowUtc)
        {
            if (Status == BlockStatus.Archived)
                return Result.Failure<ContentVersionId>
                    (ContentBlockErrors.IsArchived);

            if (string.IsNullOrEmpty(authoredBy))
                return Result.Failure<ContentVersionId>
                    (ContentBlockErrors.AuthorEmpty);

            var newVersionNumber = _versions.Max(v => v.VersionNumber) + 1;

            var newVersion = new ContentVersion(
                ContentVersionId.NewId(), newVersionNumber, body, authoredBy.Trim(), nowUtc);

            _versions.Add(newVersion);

            RaiseDomainEvents(new VersionAddedEvent(
                Id, newVersion.Id, newVersion.VersionNumber, nowUtc));

            return Result.Success(newVersion.Id);
        }

        public Result SubmitForReview(ContentVersionId versionId,
            DateTime nowUtc)
        {
            if (Status == BlockStatus.Archived)
                return Result.Failure(ContentBlockErrors.IsArchived);

            var version = _versions.SingleOrDefault(v => v.Id == versionId);
            if (version is null)
                return Result.Failure(ContentBlockErrors.VersionNotFound);

            if (version.VersionStatus != VersionStatus.Draft)
                return Result.Failure(ContentBlockErrors.VersionNotDraft);

            version.SubmitForReview();

            RaiseDomainEvents(new VersionSubmittedForReviewEvent(Id, versionId, nowUtc));

            return Result.Success();
        }

        public Result ApproveVersion(ContentVersionId versionId,
            string approvedBy,
            DateTime nowUtc)
        {
            if (Status == BlockStatus.Archived)
                return Result.Failure(ContentBlockErrors.IsArchived);

            if (string.IsNullOrEmpty(approvedBy))
                return Result.Failure(ContentBlockErrors.ApproverEmpty);

            var version = _versions.SingleOrDefault(v => v.Id == versionId);

            if (version is null)
                return Result.Failure(ContentBlockErrors.VersionNotFound);

            if (version.VersionStatus != VersionStatus.InReview)
                return Result.Failure(ContentBlockErrors.VersionNotInReview);

            var previouslyApproved = _versions.SingleOrDefault(v => v.VersionStatus == VersionStatus.Approved);
            previouslyApproved?.Supersede();

            version.Approve(approvedBy.Trim(), nowUtc);

            RaiseDomainEvents(new VersionApprovedEvent(Id, versionId, previouslyApproved?.Id, nowUtc));

            return Result.Success();
        }

        public Result Archive(DateTime nowUtc)
        {
            if (Status == BlockStatus.Archived)
                return Result.Failure(ContentBlockErrors.IsArchived);

            Status = BlockStatus.Archived;
            ArchivedAtUtc = nowUtc;

            RaiseDomainEvents(new ContentBlockArchivedEvent(Id, nowUtc));

            return Result.Success();
        }

        public static Result ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(ContentBlockErrors.NameEmpty);

            if (name.Trim().Length > NameMaxLength)
                return Result.Failure(ContentBlockErrors.NameTooLong(name.Trim().Length));

            return Result.Success();
        }
    }
}
