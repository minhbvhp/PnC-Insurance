using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyItem
{
    public long Id { get; set; }

    public string ItemName { get; set; } = null!;

    public string? ItemNameEn { get; set; }

    public virtual ICollection<PropertyPoliciesPropertyItem> PropertyPoliciesPropertyItems { get; } = new List<PropertyPoliciesPropertyItem>();
}
