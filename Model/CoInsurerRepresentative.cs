using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class CoInsurerRepresentative
{
    public long Id { get; set; }

    public long CoInsurerId { get; set; }

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string? PositionEn { get; set; }

    public string? DecisionNo { get; set; }

    public string? DecisionNoEn { get; set; }

    public virtual CoInsurer CoInsurer { get; set; } = null!;
}
