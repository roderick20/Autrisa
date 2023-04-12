
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Autrisa.Models
{
    public partial class EFContext : DbContext
    {
        public EFContext()
        {
        }

        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Bank> Banks { get; set; } = null!;
        //public virtual DbSet<Investment> Investments { get; set; } = null!;
        //public virtual DbSet<InvestmentsOperation> InvestmentsOperations { get; set; } = null!;
        //public virtual DbSet<Lending> Lendings { get; set; } = null!;
        //public virtual DbSet<LendingOperation> LendingOperations { get; set; } = null!;
        public virtual DbSet<Operation> Operations { get; set; } = null!;
        public virtual DbSet<AccountDetail> AccountDetails { get; set; } = null!;
        //public virtual DbSet<PropertiesOperation> PropertiesOperations { get; set; } = null!;
        //public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<Resource> Resources { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleResource> RoleResources { get; set; } = null!;
        public virtual DbSet<Setting> Settings { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting", "auth");
            });


            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("Bank");
            });

            //modelBuilder.Entity<InvestmentsOperation>(entity =>
            //{
            //    entity.ToTable("InvestmentsOperation");
            //});

            //modelBuilder.Entity<LendingOperation>(entity =>
            //{
            //    entity.ToTable("LendingOperation");
            //});

            //modelBuilder.Entity<PropertiesOperation>(entity =>
            //{
            //    entity.ToTable("PropertiesOperation");
            //});

            //modelBuilder.Entity<Lending>(entity =>
            //{
            //    entity.ToTable("Lending");
            //});

            //modelBuilder.Entity<Investment>(entity =>
            //{
            //    entity.ToTable("Investment");
            //});

            //modelBuilder.Entity<Property>(entity =>
            //{
            //    entity.ToTable("Property");
            //});

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.ToTable("Operation");
            });
            
            modelBuilder.Entity<AccountDetail>(entity =>
            {
                entity.ToTable("AccountDetail");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.ToTable("Resource", "auth");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "auth");
            });

            modelBuilder.Entity<RoleResource>(entity =>
            {
                entity.ToTable("RoleResource", "auth");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "auth");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole", "auth");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
