using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class CoInsurer
{
    public long Id { get; set; }

    public string TaxCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? NameEn { get; set; }

    public string? AddressEn { get; set; }

    public virtual ICollection<CoInsurerRepresentative> CoInsurerRepresentatives { get; } = new List<CoInsurerRepresentative>();
}
