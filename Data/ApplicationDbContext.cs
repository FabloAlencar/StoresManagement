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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration.Add(new BranchConfiguration());
            //modelBuilder.Configurations.Add(new BranchConfiguration());
            //modelBuilder.ApplyConfiguration.Add(new BranchConfiguration());
            //            modelBuilder.Configurations.Add(new BranchConfiguration());

            //            modelBuilder.ApplyConfiguration.Add<Branch>(new BranchConfiguration<Branch>());

            modelBuilder.ApplyConfiguration(new BranchConfiguration());

            base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}