using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyPoliciesMiscExtension
{
    public long Id { get; set; }

    public long PropertyPolicyId { get; set; }

    public long MiscExtensionId { get; set; }

    public virtual Extension MiscExtension { get; set; } = null!;

    public virtual PropertyPolicy PropertyPolicy { get; set; } = null!;
}
