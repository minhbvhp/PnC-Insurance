﻿using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Representative
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string? PositionEn { get; set; }

    public string? DecisionNo { get; set; }

    public string? DecisionNoEn { get; set; }

    public long IsDeleted { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
