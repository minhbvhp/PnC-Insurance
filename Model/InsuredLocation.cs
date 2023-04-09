using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class InsuredLocation
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public string Location { get; set; } = null!;

    public string? LocationEn { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
