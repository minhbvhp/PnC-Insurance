using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyItem
{
    public long Id { get; set; }

    public string PolicyId { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public byte[] SumInsured { get; set; } = null!;

    public string? ItemNameEn { get; set; }
}
