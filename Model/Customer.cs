using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Customer
{
    public long Id { get; set; }

    public string TaxCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Business { get; set; } = null!;

    public string? BusinessCode { get; set; }

    public string? ClientCode { get; set; }

    public string? NameEn { get; set; }

    public string? AddressEn { get; set; }

    public string? BusinessEn { get; set; }

    public virtual ICollection<InsuredLocation> InsuredLocations { get; } = new List<InsuredLocation>();

    public virtual ICollection<PropertyPolicy> PropertyPolicies { get; } = new List<PropertyPolicy>();

    public virtual ICollection<Representative> Representatives { get; } = new List<Representative>();
}
