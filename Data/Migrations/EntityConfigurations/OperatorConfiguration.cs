using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoresManagement.Models;

namespace StoresManagement.Migrations.EntityConfigurations
{
    public class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        public void Configure(EntityTypeBuilder<Operator> modelBuilder)
        {
            modelBuilder.HasKey(e => new { e.EntityId, e.UserId });

            modelBuilder.HasOne(b => b.Entity)
                .WithMany(e => e.Operators);

            modelBuilder.HasOne(b => b.Contact)
                .WithOne(c => c.Operators)
                .HasForeignKey<Operator>(b => b.ContactId);
        }
    }
}