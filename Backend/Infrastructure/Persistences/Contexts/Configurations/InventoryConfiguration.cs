using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventory")
                .HasKey(i => new { i.PK_STORE, i.PK_PRODUCT });
            builder.Property(i => i.PK_STORE)
                .IsRequired();
            builder.Property(i => i.PK_PRODUCT)
                .IsRequired();
        }
    }
}
