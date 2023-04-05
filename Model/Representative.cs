using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Representative
{
    public long Id { get; set; }

    public string CompanyTaxCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string? PositionEn { get; set; }

    public string? DecisionNo { get; set; }

    public string? DecisionNoEn { get; set; }

    public virtual Customer CompanyTaxCodeNavigation { get; set; } = null!;
}
