using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class FnEdeductible
{
    public long Id { get; set; }

    public string SumInsured { get; set; } = null!;

    public long DeductibleAmount { get; set; }
}
