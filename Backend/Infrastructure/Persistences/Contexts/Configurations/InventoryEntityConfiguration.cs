using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InventoryEntityConfiguration : IEntityTypeConfiguration<InventoryEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryEntity> builder)
        {
            builder.ToTable("INVENTORY")
                .HasKey(i => new { i.IdStore, i.IdProduct });

            builder.Property(i => i.IdStore)
                .HasColumnName("PK_STORE")
                .IsRequired();

            builder.Property(i => i.IdProduct)
                .HasColumnName("PK_PRODUCT")
                .IsRequired();

            builder.Property(i => i.Stock)
                .HasColumnName("STOCK")
                .IsRequired();

            builder.Property(i => i.Price)
                .HasColumnName("PRICE")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(e => e.AuditUpdateUser)
                .HasColumnName("AUDIT_UPDATE_USER");

            builder.Property(e => e.AuditUpdateDate)
                .HasColumnName("AUDIT_UPDATE_DATE");

            builder.HasOne(s => s.Store)
                .WithMany(i => i.Inventory)
                .HasForeignKey(i => i.IdStore)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(i => i.Inventory)
                .HasForeignKey(i => i.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
