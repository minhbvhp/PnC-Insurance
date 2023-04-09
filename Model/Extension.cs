using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Extension
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public string DescriptionEn { get; set; } = null!;
}
