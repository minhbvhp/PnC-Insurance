using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyGeneralExtension
{
    public long Id { get; set; }

    public long ExtensionId { get; set; }

    public long IsDeleted { get; set; }

    public virtual Extension Extension { get; set; } = null!;
}
