using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Department
{
    public long Urn { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Agent> Agents { get; } = new List<Agent>();

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
