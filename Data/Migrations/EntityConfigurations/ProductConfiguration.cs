using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoresManagement.Models;

namespace StoresManagement.Data.Migrations.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> modelBuilder)
        {
            modelBuilder.HasMany(b => b.PurchaseItems)
                .WithOne(e => e.Product)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder
                .Property(p => p.Active)
                .HasDefaultValue(true);
        }
    }
}