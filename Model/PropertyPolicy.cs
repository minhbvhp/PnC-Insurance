using System;
using System.Collections.Generic;

namespace PnC_Insurance.Model;

public partial class PropertyPolicy
{
    public long Id { get; set; }

    public string PolicyNo { get; set; } = null!;

    public long CusomerId { get; set; }

    public string DateIssue { get; set; } = null!;

    public string FromDate { get; set; } = null!;

    public string ToDate { get; set; } = null!;

    public long ClassOfInsuranceId { get; set; }

    public long SumInsured { get; set; }

    public string FnEpremiumRate { get; set; } = null!;

    public string ArpremiumRate { get; set; } = null!;

    public long FnEpremium { get; set; }

    public long Arpremium { get; set; }

    public long Vat { get; set; }

    public long TotalDue { get; set; }

    public long FneDeductibleRate { get; set; }

    public long FneDeductibleAmount { get; set; }

    public long ArdeductibleRate { get; set; }

    public long ArdeductibleAmount { get; set; }

    public string ArdeductibleMisc { get; set; } = null!;

    public long Confirm { get; set; }

    public long DepartmentId { get; set; }

    public string DueDate { get; set; } = null!;

    public long IsDeleted { get; set; }

    public virtual Customer Cusomer { get; set; } = null!;

    public virtual ICollection<PropertyPoliciesCoInsurer> PropertyPoliciesCoInsurers { get; } = new List<PropertyPoliciesCoInsurer>();

    public virtual ICollection<PropertyPoliciesExtension> PropertyPoliciesExtensions { get; } = new List<PropertyPoliciesExtension>();

    public virtual ICollection<PropertyPoliciesMiscExtension> PropertyPoliciesMiscExtensions { get; } = new List<PropertyPoliciesMiscExtension>();

    public virtual ICollection<PropertyPoliciesPropertyItem> PropertyPoliciesPropertyItems { get; } = new List<PropertyPoliciesPropertyItem>();
}
