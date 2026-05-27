using System;
using System.Collections.Generic;
using System.Text;

namespace SaasContentLibrary.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
