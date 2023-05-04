using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class Department
{
    public long Id { get; set; }

    public string Urn { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long IsDeleted { get; set; }

    public virtual ICollection<Agent> Agents { get; } = new List<Agent>();

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
