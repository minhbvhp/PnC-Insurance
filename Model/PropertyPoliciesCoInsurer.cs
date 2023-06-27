using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyPoliciesCoInsurer
{
    public long Id { get; set; }

    public long PropertyPolicyId { get; set; }

    public long CoInsurerId { get; set; }

    public string Rate { get; set; } = null!;

    public string Fee { get; set; } = null!;

    public virtual CoInsurer CoInsurer { get; set; } = null!;

    public virtual PropertyPolicy PropertyPolicy { get; set; } = null!;
}
