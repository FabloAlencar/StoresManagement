using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoresManagement.Models;

namespace StoresManagement.Migrations.EntityConfigurations
{
    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> modelBuilder)
        {
            modelBuilder.HasOne(b => b.Purchase)
                .WithMany(e => e.PurchaseItems);

            modelBuilder.HasOne(b => b.Product)
                .WithOne(e => e.PurchaseItem);
        }
    }
}