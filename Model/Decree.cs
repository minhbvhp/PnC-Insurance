using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Decree
{
    public long Id { get; set; }

    public string Content { get; set; } = null!;

    public string ContentEn { get; set; } = null!;
}
