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

    public string FneDeductibleRate { get; set; } = null!;

    public long FneDeductibleAmount { get; set; }

    public string ArdeductibleRate { get; set; } = null!;

    public long ArdeductibleAmount { get; set; }

    public long Confirm { get; set; }

    public long DepartmentId { get; set; }

    public string? MiscExtensions { get; set; }

    public string? MiscExtensionsEn { get; set; }

    public long IsDeleted { get; set; }

    public virtual Customer Cusomer { get; set; } = null!;

    public virtual ICollection<PropertyPoliciesCoInsurer> PropertyPoliciesCoInsurers { get; } = new List<PropertyPoliciesCoInsurer>();

    public virtual ICollection<PropertyPoliciesExtension> PropertyPoliciesExtensions { get; } = new List<PropertyPoliciesExtension>();

    public virtual ICollection<PropertyPoliciesPropertyItem> PropertyPoliciesPropertyItems { get; } = new List<PropertyPoliciesPropertyItem>();
}
