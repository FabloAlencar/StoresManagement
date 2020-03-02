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
            //.HasForeignKey<Branch>(b => b.EntityId);

            modelBuilder.HasOne(b => b.Contact)
                .WithOne(c => c.Branch)
                .HasForeignKey<Branch>(b => b.ContactId);

            //HasRequired(b => b.Entity)
            //.WithMany(e => e.Branches)
            //.HasForeignKey(b => b.EntityId)
            //.WillCascadeOnDelete(false);

            //HasRequired(b => b.Contact)
            //    .WithOptional(con => con.Branch)
            //    .Map(b => b.MapKey("ContactId"));
        }
    }
}