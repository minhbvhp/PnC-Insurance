using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class CustomersInsuredLocation
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public long InsuredLocationId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual InsuredLocation InsuredLocation { get; set; } = null!;
}
