using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoresManagement.Models;

namespace StoresManagement.Migrations.EntityConfigurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> modelBuilder)
        {
            modelBuilder.HasOne(b => b.Entity)
                .WithMany(e => e.Customers);

            modelBuilder.HasOne(b => b.Contact)
                .WithOne(c => c.Customer)
                .HasForeignKey<Customer>(b => b.ContactId);

            modelBuilder.HasMany(c => c.Purchases)
                .WithOne(p => p.Customer)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }
}