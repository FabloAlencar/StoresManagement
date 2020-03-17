using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Migrations.EntityConfigurations;
using StoresManagement.Models;
using System.Reflection;

namespace StoresManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Entity> Entities { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BranchConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}