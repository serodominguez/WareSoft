using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InboundTransferDetailsEntityConfiguration : IEntityTypeConfiguration<InboundTransferDetailsEntity>
    {
        public void Configure(EntityTypeBuilder<InboundTransferDetailsEntity> builder)
        {
            builder.ToTable("INBOUND_TRANSFER_DETAILS")
                .HasKey(d => new { d.IdInbound, d.Item });

            builder.Property(d => d.IdInbound)
                .HasColumnName("PK_INBOUND")
                .IsRequired();

            builder.Property(d => d.Item)
                .HasColumnName("ITEM")
                .IsRequired();

            builder.Property(d => d.IdProduct)
                .HasColumnName("PK_PRODUCT")
                .IsRequired();

            builder.Property(d => d.Quantity)
                .HasColumnName("QUANTITY")
                .IsRequired();

            builder.Property(d => d.UnitPrice)
                .HasColumnName("UNIT_PRICE")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.TotalPrice)
                .HasColumnName("TOTAL_PRICE")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.HasOne(i => i.InboundTransfer)
                .WithMany(d => d.InboundTransferDetails)
                .HasForeignKey(d => d.IdInbound)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(d => d.InboundTransferDetails)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
