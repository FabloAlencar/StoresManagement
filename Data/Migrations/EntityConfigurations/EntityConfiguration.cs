using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoresManagement.Models;

namespace StoresManagement.Data.Migrations.EntityConfigurations
{
    public class EntityConfiguration : IEntityTypeConfiguration<Entity>
    {
        public void Configure(EntityTypeBuilder<Entity> modelBuilder)
        {
            modelBuilder
                .Property(p => p.Active)
                .HasDefaultValue(true);
        }
    }
}