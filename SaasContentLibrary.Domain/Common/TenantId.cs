using System;
using System.Collections.Generic;
using System.Text;

namespace SaasContentLibrary.Domain.Common
{
    public readonly record struct TenantId(Guid Value)
    {
        public static TenantId NewId() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }
}
