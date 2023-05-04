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

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Decree> Decrees { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Extension> Extensions { get; set; }

    public virtual DbSet<InsuredLocation> InsuredLocations { get; set; }

    public virtual DbSet<PropertyItem> PropertyItems { get; set; }

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

        modelBuilder.Entity<InsuredLocation>(entity =>
        {
            entity.ToTable("InsuredLocation");

            entity.HasIndex(e => e.Id, "IX_InsuredLocation_Id").IsUnique();

            entity.HasIndex(e => new { e.CustomerId, e.Location }, "available_insured_location_unique").IsUnique();

            entity.Property(e => e.LocationEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("LocationEN");

            entity.HasOne(d => d.Customer).WithMany(p => p.InsuredLocations)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PropertyItem>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_PropertyItems_Id").IsUnique();

            entity.HasIndex(e => new { e.PolicyId, e.ItemName }, "available_property_item_unique").IsUnique();

            entity.Property(e => e.ItemName).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.ItemNameEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("ItemNameEN");
            entity.Property(e => e.PolicyId).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.SumInsured)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
        });

        modelBuilder.Entity<PropertyPolicy>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_PropertyPolicies_Id").IsUnique();

            entity.Property(e => e.Ardeductible)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("ARDeductible");
            entity.Property(e => e.ArpremiumRate)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("ARPremiumRate");
            entity.Property(e => e.DateIssue).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.FnEpremiumRate)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("FnEPremiumRate");
            entity.Property(e => e.FneDeductible).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.FromDate).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.InsuredLocationId).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.PolicyNo).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.Premium)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.ToDate).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.TotalDue)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.Vat)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC")
                .HasColumnName("VAT");

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
