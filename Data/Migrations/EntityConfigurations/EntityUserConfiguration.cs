using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoresManagement.Models;

namespace StoresManagement.Migrations.EntityConfigurations
{
    public class EntityUserConfiguration : IEntityTypeConfiguration<EntityUser>
    {
        public void Configure(EntityTypeBuilder<EntityUser> modelBuilder)
        {
            modelBuilder.HasKey(e => new { e.EntityId, e.UserId });

            modelBuilder.HasOne(b => b.Entity)
                .WithMany(e => e.EntityUsers);
        }
    }
}