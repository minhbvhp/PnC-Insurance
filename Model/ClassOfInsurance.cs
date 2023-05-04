using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class ClassOfInsurance
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long? TermId { get; set; }

    public long? DecreeId { get; set; }

    public long IsDeleted { get; set; }
}
