using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class FnEpremiumRate
{
    public long Id { get; set; }

    public string Category { get; set; } = null!;

    public string DeductibleKind { get; set; } = null!;

    public string Rate { get; set; } = null!;

    public string? GroupDescription { get; set; }
}
