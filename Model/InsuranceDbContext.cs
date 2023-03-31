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

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("DataSource=.\\Data\\Insurance DB.db");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
