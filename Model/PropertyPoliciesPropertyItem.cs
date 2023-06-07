using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyPoliciesPropertyItem
{
    public long Id { get; set; }

    public long PropertyPolicyId { get; set; }

    public long PropertyItemId { get; set; }

    public long SumInsured { get; set; }

    public virtual PropertyItem PropertyItem { get; set; } = null!;

    public virtual PropertyPolicy PropertyPolicy { get; set; } = null!;
}
