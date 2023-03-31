﻿using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Employee
{
    public long Urn { get; set; }

    public string FullName { get; set; } = null!;

    public long DeptUrn { get; set; }
}
