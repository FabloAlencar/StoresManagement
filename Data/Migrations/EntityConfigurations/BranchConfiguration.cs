using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoresManagement.Models;

namespace StoresManagement.Migrations.EntityConfigurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> modelBuilder)
        {
            modelBuilder.HasOne(b => b.Entity)
                .WithMany(e => e.Branches);

            modelBuilder.HasOne(b => b.Contact)
                .WithOne(c => c.Branch)
                .HasForeignKey<Branch>(b => b.ContactId);

            modelBuilder.HasMany(b => b.Products)
                .WithOne(p => p.Branch);

            modelBuilder.HasMany(b => b.Purchases)
                .WithOne(p => p.Branch)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }
}