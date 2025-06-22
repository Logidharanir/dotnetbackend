using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyAPI.Models
{
    public partial class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext() { }

        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options) { }

        // ── DbSets ───────────────────────────────────────────────────────
        public virtual DbSet<Department> Departments { get; set; } = default!;
        public virtual DbSet<Employee>   Employees   { get; set; } = default!;

        // ❌  REMOVED hard-coded UseSqlServer
        // The connection is now supplied by Program.cs → UseNpgsql()

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── departments ──────────────────────────────────────────────
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");
                entity.HasKey(e => e.DepartmentId);
                entity.Property(e => e.DepartmentId)
                      .ValueGeneratedNever()
                      .HasColumnName("department_id");
                entity.Property(e => e.DepartmentName)
                      .HasMaxLength(50)
                      .HasColumnName("department_name");
            });

            // ── employees ────────────────────────────────────────────────
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.EmployeeId)
                      .ValueGeneratedNever()
                      .HasColumnName("employee_id");
                entity.Property(e => e.Name)
                      .HasMaxLength(100)
                      .HasColumnName("name");
                entity.Property(e => e.Age).HasColumnName("age");
                entity.Property(e => e.Salary)
                      .HasColumnType("numeric(10,2)")
                      .HasColumnName("salary");
                entity.Property(e => e.DepartmentId).HasColumnName("department_id");
                entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
