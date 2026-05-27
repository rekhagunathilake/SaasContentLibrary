using System;
using System.Collections.Generic;
using System.Text;

namespace SaasContentLibrary.Domain
{
    public record ContentVersion
    {
        public int Id { get; set; }
        public int VersionNumber { get; set; }
        public string Body { get; set; }
        public Status Status { get; set; }
        Guid AuthoredBy { get; set; }
        Guid ApprovedBy { get; set; }
        DateTime AuthoredAt { get; set; }
        DateTime ApprovedAt { get; set; }
    }
}
