using SaasContentLibrary.Domain.Common;

namespace SaasContentLibrary.Domain.ContentBlocks
{
    public sealed class ContentVersion : Entity<ContentVersionId>
    {
        public int VersionNumber { get; private set; }
        public ContentBody ContentBody { get; private set; } = null!;
        public VersionStatus VersionStatus { get; private set; }
        public string AuthoredBy { get; private set; } = null!;
        public DateTime AuthoredOnUtc { get; private set; }
        public string? ApprovedBy { get; private set; }
        public DateTime? ApprovedOnUtc { get; private set; }

        private ContentVersion() { } // For EF Core

        internal ContentVersion(
            ContentVersionId id,
            int versionNumber,
            ContentBody contentBody,
            string authoredBy,
            DateTime authoredOnUtc) : base(id)
        {
            VersionNumber = versionNumber;
            ContentBody = contentBody;
            VersionStatus = VersionStatus.Draft;
            AuthoredBy = authoredBy;
            AuthoredOnUtc = authoredOnUtc;
        }

        internal void SubmitForReview() => VersionStatus = VersionStatus.InReview;

        internal void Approve(string approvedBy, DateTime ApprovedAtUtc)
        {
            VersionStatus = VersionStatus.Approved;
            ApprovedBy = approvedBy;
            ApprovedOnUtc = ApprovedAtUtc;
        }

        internal void Supersede() => VersionStatus = VersionStatus.Superseded;
    }
}
