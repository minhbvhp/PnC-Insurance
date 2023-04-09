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

    public long InsuredLocationId { get; set; }

    public long ClassOfInsuranceId { get; set; }

    public double FnEpremiumRate { get; set; }

    public double ArpremiumRate { get; set; }

    public byte[] Premium { get; set; } = null!;

    public byte[] Vat { get; set; } = null!;

    public byte[] TotalDue { get; set; } = null!;

    public string FneDeductible { get; set; } = null!;

    public string Ardeductible { get; set; } = null!;

    public long TermId { get; set; }

    public long Confirm { get; set; }

    public virtual Customer Cusomer { get; set; } = null!;
}
