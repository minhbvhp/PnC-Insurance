using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyPoliciesExtension
{
    public long Id { get; set; }

    public long PropertyPolicyId { get; set; }

    public long ExtensionId { get; set; }

    public string? Sublimit { get; set; }

    public string? SublimitEn { get; set; }

    public virtual Extension Extension { get; set; } = null!;

    public virtual PropertyPolicy PropertyPolicy { get; set; } = null!;
}
