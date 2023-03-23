using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Autrisa.Models
{
    public partial class Autrisa_394Context : DbContext
    {
        public Autrisa_394Context()
        {
        }

        public Autrisa_394Context(DbContextOptions<Autrisa_394Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Investment> Investments { get; set; } = null!;
        public virtual DbSet<InvestmentsOperation> InvestmentsOperations { get; set; } = null!;
        public virtual DbSet<Lending> Lendings { get; set; } = null!;
        public virtual DbSet<LendingOperation> LendingOperations { get; set; } = null!;
        public virtual DbSet<Operation> Operations { get; set; } = null!;
        public virtual DbSet<PropertiesOperation> PropertiesOperations { get; set; } = null!;
        public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<Resource> Resources { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleResource> RoleResources { get; set; } = null!;
        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=3.131.217.111;Database=Autrisa_394;user id=user_Autrisa_394;password=!g78Z8-H;encrypt=false");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountNumber).HasMaxLength(50);

                entity.Property(e => e.AccountType).HasMaxLength(50);

                entity.Property(e => e.Amount).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasComment("0: Sol, 1: Dollar");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PreviousRemaining).HasColumnType("decimal(16, 2)");
            });

            modelBuilder.Entity<Investment>(entity =>
            {
                entity.ToTable("Investment");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Customer).HasMaxLength(50);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.OperationAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OperationDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Investment_Account");
            });

            modelBuilder.Entity<InvestmentsOperation>(entity =>
            {
                entity.ToTable("InvestmentsOperation");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.Investment)
                    .WithMany(p => p.InvestmentsOperations)
                    .HasForeignKey(d => d.InvestmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_InvestmentsOperation_Investment");
            });

            modelBuilder.Entity<Lending>(entity =>
            {
                entity.ToTable("Lending");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Customer).HasMaxLength(50);

                entity.Property(e => e.LendDate).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Lendings)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lending_Account");
            });

            modelBuilder.Entity<LendingOperation>(entity =>
            {
                entity.ToTable("LendingOperation");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.OperationDate).HasColumnType("datetime");

                entity.HasOne(d => d.Lending)
                    .WithMany(p => p.LendingOperations)
                    .HasForeignKey(d => d.LendingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LendingOperation_Lending");
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.ToTable("Operation");

                entity.Property(e => e.Concept).HasMaxLength(300);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(300);

                entity.Property(e => e.Income).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Modality).HasComment("0: Check, 1: Transfer, 2...");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.OperationDate).HasColumnType("datetime");

                entity.Property(e => e.Outcome).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Type).HasComment("0: Income, 1: Outcome, 2: Remaining");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Operation_Account");
            });

            modelBuilder.Entity<PropertiesOperation>(entity =>
            {
                entity.ToTable("PropertiesOperation");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.OperationDate).HasColumnType("datetime");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.PropertiesOperations)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PropertiesOperation_Property");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Property");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Number).HasMaxLength(50);

                entity.Property(e => e.Participation).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Property_Property");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.ToTable("Resource", "auth");

                entity.Property(e => e.Action).HasMaxLength(200);

                entity.Property(e => e.Area).HasMaxLength(200);

                entity.Property(e => e.Attributes).HasMaxLength(200);

                entity.Property(e => e.Controller).HasMaxLength(200);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "auth");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<RoleResource>(entity =>
            {
                entity.ToTable("RoleResource", "auth");

                entity.Property(e => e.Action).HasMaxLength(200);

                entity.Property(e => e.Area).HasMaxLength(200);

                entity.Property(e => e.Attributes).HasMaxLength(200);

                entity.Property(e => e.Controller).HasMaxLength(200);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Icon).HasMaxLength(200);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.RoleResources)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleResource_Resource");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleResources)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleResource_Role");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting", "auth");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Key).HasMaxLength(50);

                entity.Property(e => e.Modified).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "auth");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.LastAccess).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Password).HasMaxLength(150);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole", "auth");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserRole_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
