using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Employee
{
    public long Id { get; set; }

    public long Urn { get; set; }

    public string FullName { get; set; } = null!;

    public long DeptId { get; set; }

    public virtual Department Dept { get; set; } = null!;
}
