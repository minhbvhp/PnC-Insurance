using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyPoliciesInsuredLocation
{
    public long Id { get; set; }

    public long PropertyPolicyId { get; set; }

    public long InsuredLocationId { get; set; }
}
