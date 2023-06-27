using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PnC_Insurance.Model;

public partial class InsuranceDbContext : DbContext
{
    public InsuranceDbContext()
    {
    }

    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agent> Agents { get; set; }

    public virtual DbSet<ClassOfInsurance> ClassOfInsurances { get; set; }

    public virtual DbSet<CoInsurer> CoInsurers { get; set; }

    public virtual DbSet<CoInsurerRepresentative> CoInsurerRepresentatives { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomersInsuredLocation> CustomersInsuredLocations { get; set; }

    public virtual DbSet<Decree> Decrees { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Extension> Extensions { get; set; }

    public virtual DbSet<FnEdeductible> FnEdeductibles { get; set; }

    public virtual DbSet<FnEpremiumRate> FnEpremiumRates { get; set; }

    public virtual DbSet<InsuredLocation> InsuredLocations { get; set; }

    public virtual DbSet<PropertyGeneralExtension> PropertyGeneralExtensions { get; set; }

    public virtual DbSet<PropertyItem> PropertyItems { get; set; }

    public virtual DbSet<PropertyPoliciesCoInsurer> PropertyPoliciesCoInsurers { get; set; }

    public virtual DbSet<PropertyPoliciesExtension> PropertyPoliciesExtensions { get; set; }

    public virtual DbSet<PropertyPoliciesInsuredLocation> PropertyPoliciesInsuredLocations { get; set; }

    public virtual DbSet<PropertyPoliciesPropertyItem> PropertyPoliciesPropertyItems { get; set; }

    public virtual DbSet<PropertyPolicy> PropertyPolicies { get; set; }

    public virtual DbSet<Representative> Representatives { get; set; }

    public virtual DbSet<Term> Terms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("DataSource=.\\Data\\Insurance DB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Agents_Id").IsUnique();

            entity.HasIndex(e => e.Urn, "available_agent_unique").IsUnique();

            entity.Property(e => e.Urn).HasColumnName("URN");

            entity.HasOne(d => d.Dept).WithMany(p => p.Agents)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ClassOfInsurance>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_ClassOfInsurances_Id").IsUnique();

            entity.HasIndex(e => new { e.Name, e.TermId, e.DecreeId }, "available_class_of_insurance_unique").IsUnique();

            entity.HasOne(d => d.Decree).WithMany(p => p.ClassOfInsurances).HasForeignKey(d => d.DecreeId);

            entity.HasOne(d => d.Term).WithMany(p => p.ClassOfInsurances).HasForeignKey(d => d.TermId);
        });

        modelBuilder.Entity<CoInsurer>(entity =>
        {
            entity.ToTable("CoInsurer");

            entity.HasIndex(e => e.Id, "IX_CoInsurer_Id").IsUnique();

            entity.Property(e => e.AddressEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("AddressEN");
            entity.Property(e => e.NameEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("NameEN");
        });

        modelBuilder.Entity<CoInsurerRepresentative>(entity =>
        {
            entity.ToTable("CoInsurer_Representatives");

            entity.HasIndex(e => e.Id, "IX_CoInsurer_Representatives_Id").IsUnique();

            entity.Property(e => e.DecisionNo).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.DecisionNoEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("DecisionNoEN");
            entity.Property(e => e.PositionEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("PositionEN");

            entity.HasOne(d => d.CoInsurer).WithMany(p => p.CoInsurerRepresentatives)
                .HasForeignKey(d => d.CoInsurerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Customers_Id").IsUnique();

            entity.HasIndex(e => e.TaxCode, "available_customer_unique").IsUnique();

            entity.Property(e => e.AddressEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("AddressEN");
            entity.Property(e => e.BusinessCode).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.BusinessEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("BusinessEN");
            entity.Property(e => e.ClientCode).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.NameEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("NameEN");
        });

        modelBuilder.Entity<CustomersInsuredLocation>(entity =>
        {
            entity.ToTable("Customers_InsuredLocations");

            entity.HasIndex(e => e.Id, "IX_Customers_InsuredLocations_Id").IsUnique();

            entity.HasIndex(e => new { e.CustomerId, e.InsuredLocationId }, "IX_Customers_InsuredLocations_CustomerId_InsuredLocationId").IsUnique();

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomersInsuredLocations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InsuredLocation).WithMany(p => p.CustomersInsuredLocations)
                .HasForeignKey(d => d.InsuredLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Decree>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Decrees_Id").IsUnique();

            entity.Property(e => e.Content).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.ContentEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("ContentEN");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Departments_Id").IsUnique();

            entity.HasIndex(e => e.Urn, "available_department_unique").IsUnique();

            entity.Property(e => e.Urn).HasColumnName("URN");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Employees_Id").IsUnique();

            entity.HasIndex(e => e.Urn, "available_employee_unique").IsUnique();

            entity.Property(e => e.Urn).HasColumnName("URN");

            entity.HasOne(d => d.Dept).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Extension>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Extensions_Id").IsUnique();

            entity.HasIndex(e => e.Code, "available_extension_unique").IsUnique();

            entity.Property(e => e.Description).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.DescriptionEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("DescriptionEN");
            entity.Property(e => e.Name).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.NameEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("NameEN");
        });

        modelBuilder.Entity<FnEdeductible>(entity =>
        {
            entity.ToTable("FnEDeductible");

            entity.HasIndex(e => e.Id, "IX_FnEDeductible_Id").IsUnique();
        });

        modelBuilder.Entity<FnEpremiumRate>(entity =>
        {
            entity.ToTable("FnEPremiumRate");

            entity.HasIndex(e => e.Id, "IX_FnEPremiumRate_Id").IsUnique();
        });

        modelBuilder.Entity<InsuredLocation>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_InsuredLocations_Id").IsUnique();

            entity.HasIndex(e => e.Location, "available_insured_location_unique").IsUnique();

            entity.Property(e => e.LocationEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("LocationEN");
        });

        modelBuilder.Entity<PropertyGeneralExtension>(entity =>
        {
            entity.HasIndex(e => e.ExtensionId, "IX_PropertyGeneralExtensions_ExtensionId").IsUnique();

            entity.HasIndex(e => e.Id, "IX_PropertyGeneralExtensions_Id").IsUnique();

            entity.HasIndex(e => e.ExtensionId, "available_general_extensions_unique").IsUnique();

            entity.HasOne(d => d.Extension).WithOne(p => p.PropertyGeneralExtension)
                .HasForeignKey<PropertyGeneralExtension>(d => d.ExtensionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PropertyItem>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_PropertyItems_Id").IsUnique();

            entity.Property(e => e.ItemName).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.ItemNameEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("ItemNameEN");
        });

        modelBuilder.Entity<PropertyPoliciesCoInsurer>(entity =>
        {
            entity.ToTable("PropertyPolicies_CoInsurer");

            entity.HasIndex(e => e.Id, "IX_PropertyPolicies_CoInsurer_Id").IsUnique();

            entity.HasIndex(e => new { e.PropertyPolicyId, e.CoInsurerId }, "IX_PropertyPolicies_CoInsurer_PropertyPolicyId_CoInsurerId").IsUnique();

            entity.HasOne(d => d.CoInsurer).WithMany(p => p.PropertyPoliciesCoInsurers)
                .HasForeignKey(d => d.CoInsurerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PropertyPolicy).WithMany(p => p.PropertyPoliciesCoInsurers)
                .HasForeignKey(d => d.PropertyPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PropertyPoliciesExtension>(entity =>
        {
            entity.ToTable("PropertyPolicies_Extensions");

            entity.HasIndex(e => e.Id, "IX_PropertyPolicies_Extensions_Id").IsUnique();

            entity.Property(e => e.SublimitEn).HasColumnName("SublimitEN");

            entity.HasOne(d => d.Extension).WithMany(p => p.PropertyPoliciesExtensions)
                .HasForeignKey(d => d.ExtensionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PropertyPolicy).WithMany(p => p.PropertyPoliciesExtensions)
                .HasForeignKey(d => d.PropertyPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PropertyPoliciesInsuredLocation>(entity =>
        {
            entity.ToTable("PropertyPolicies_InsuredLocations");

            entity.HasIndex(e => e.Id, "IX_PropertyPolicies_InsuredLocations_Id").IsUnique();

            entity.HasIndex(e => new { e.PropertyPolicyId, e.InsuredLocationId }, "IX_PropertyPolicies_InsuredLocations_PropertyPolicyId_InsuredLocationId").IsUnique();
        });

        modelBuilder.Entity<PropertyPoliciesPropertyItem>(entity =>
        {
            entity.ToTable("PropertyPolicies_PropertyItems");

            entity.HasIndex(e => e.Id, "IX_PropertyPolicies_PropertyItems_Id").IsUnique();

            entity.HasIndex(e => new { e.PropertyPolicyId, e.PropertyItemId }, "IX_PropertyPolicies_PropertyItems_PropertyPolicyId_PropertyItemId").IsUnique();

            entity.HasOne(d => d.PropertyItem).WithMany(p => p.PropertyPoliciesPropertyItems)
                .HasForeignKey(d => d.PropertyItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PropertyPolicy).WithMany(p => p.PropertyPoliciesPropertyItems)
                .HasForeignKey(d => d.PropertyPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PropertyPolicy>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_PropertyPolicies_Id").IsUnique();

            entity.Property(e => e.ArdeductibleAmount).HasColumnName("ARDeductibleAmount");
            entity.Property(e => e.ArdeductibleRate)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("ARDeductibleRate");
            entity.Property(e => e.Arpremium).HasColumnName("ARPremium");
            entity.Property(e => e.ArpremiumRate)
                .HasDefaultValueSql("0")
                .HasColumnName("ARPremiumRate");
            entity.Property(e => e.DateIssue).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.FnEpremium).HasColumnName("FnEPremium");
            entity.Property(e => e.FnEpremiumRate)
                .HasDefaultValueSql("0")
                .HasColumnName("FnEPremiumRate");
            entity.Property(e => e.FneDeductibleRate).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.FromDate).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.MiscExtensionsEn).HasColumnName("MiscExtensionsEN");
            entity.Property(e => e.PolicyNo).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.ToDate).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.Vat).HasColumnName("VAT");

            entity.HasOne(d => d.Cusomer).WithMany(p => p.PropertyPolicies)
                .HasForeignKey(d => d.CusomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Representative>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Representatives_Id").IsUnique();

            entity.HasIndex(e => new { e.CustomerId, e.FullName, e.Position }, "available_representative_unique").IsUnique();

            entity.Property(e => e.DecisionNo).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.DecisionNoEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("DecisionNoEN");
            entity.Property(e => e.PositionEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("PositionEN");

            entity.HasOne(d => d.Customer).WithMany(p => p.Representatives)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Term>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Terms_Id").IsUnique();

            entity.Property(e => e.Content).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.ContentEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("ContentEN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
