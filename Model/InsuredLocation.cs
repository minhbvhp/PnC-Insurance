using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class InsuredLocation
{
    public long Id { get; set; }

    public string Location { get; set; } = null!;

    public string? LocationEn { get; set; }

    public long IsDeleted { get; set; }

    public virtual ICollection<CustomersInsuredLocation> CustomersInsuredLocations { get; } = new List<CustomersInsuredLocation>();
}
