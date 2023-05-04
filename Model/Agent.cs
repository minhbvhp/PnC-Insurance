using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Agent
{
    public long Id { get; set; }

    public string Urn { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public long DeptId { get; set; }

    public long IsDeleted { get; set; }

    public virtual Department Dept { get; set; } = null!;
}
