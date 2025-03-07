﻿using Microsoft.EntityFrameworkCore;
using WebApplication0.Models;

namespace WebApplication0.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }
        public DbSet<Invitation> Invitations { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCompany>()
                .HasKey(uc => new { uc.UserId, uc.CompanyId });

            modelBuilder.Entity<UserCompany>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCompanies)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserCompany>()
                .HasOne(uc => uc.Company)
                .WithMany(c => c.UserCompanies)
                .HasForeignKey(uc => uc.CompanyId);

            modelBuilder.Entity<Invitation>()
              .HasKey(i => i.Id);

            modelBuilder.Entity<Invitation>()
                .Property(i => i.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Invitation>()
                .Property(i => i.Token)
                .IsRequired();

            modelBuilder.Entity<Invitation>()
                .Property(i => i.EmpresasSelecionadas)
                .IsRequired();
        }
    }
}
