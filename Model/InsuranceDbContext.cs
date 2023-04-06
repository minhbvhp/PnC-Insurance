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

    public virtual DbSet<Representative> Representatives { get; set; }

    public virtual DbSet<Term> Terms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("DataSource=.\\.\\Data\\Insurance DB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.Urn);

            entity.HasIndex(e => e.Urn, "IX_Agents_URN").IsUnique();

            entity.Property(e => e.Urn)
                .ValueGeneratedNever()
                .HasColumnName("URN");
            entity.Property(e => e.DeptUrn).HasColumnName("DeptURN");

            entity.HasOne(d => d.DeptUrnNavigation).WithMany(p => p.Agents)
                .HasForeignKey(d => d.DeptUrn)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ClassOfInsurance>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_ClassOfInsurances_Id").IsUnique();

            entity.HasIndex(e => new { e.Name, e.TermId, e.DecreeId }, "IX_ClassOfInsurances_Name_TermId_DecreeId").IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.TaxCode);

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
            entity.HasKey(e => e.Urn);

            entity.HasIndex(e => e.Urn, "IX_Departments_URN").IsUnique();

            entity.Property(e => e.Urn)
                .ValueGeneratedNever()
                .HasColumnName("URN");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Urn);

            entity.HasIndex(e => e.Urn, "IX_Employees_URN").IsUnique();

            entity.Property(e => e.Urn)
                .ValueGeneratedNever()
                .HasColumnName("URN");
            entity.Property(e => e.DeptUrn).HasColumnName("DeptURN");

            entity.HasOne(d => d.DeptUrnNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DeptUrn)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Extension>(entity =>
        {
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

            entity.HasIndex(e => new { e.CompanyTaxCode, e.Location }, "IX_InsuredLocation_CompanyTaxCode_Location").IsUnique();

            entity.Property(e => e.LocationEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("LocationEN");

            entity.HasOne(d => d.CompanyTaxCodeNavigation).WithMany(p => p.InsuredLocations)
                .HasForeignKey(d => d.CompanyTaxCode)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Representative>(entity =>
        {
            entity.ToTable("Representative");

            entity.HasIndex(e => e.Id, "IX_Representative_Id").IsUnique();

            entity.HasIndex(e => new { e.CompanyTaxCode, e.FullName, e.Position }, "IX_Representative_CompanyTaxCode_FullName_Position").IsUnique();

            entity.Property(e => e.DecisionNo).HasDefaultValueSql("'Chưa thiết lập'");
            entity.Property(e => e.DecisionNoEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("DecisionNoEN");
            entity.Property(e => e.PositionEn)
                .HasDefaultValueSql("'Chưa thiết lập'")
                .HasColumnName("PositionEN");

            entity.HasOne(d => d.CompanyTaxCodeNavigation).WithMany(p => p.Representatives)
                .HasForeignKey(d => d.CompanyTaxCode)
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
