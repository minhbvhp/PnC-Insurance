using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Extension
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public string DescriptionEn { get; set; } = null!;
}
