using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class OutboundTransferDetailsEntityConfiguration : IEntityTypeConfiguration<OutboundTransferDetailsEntity>
    {
        public void Configure(EntityTypeBuilder<OutboundTransferDetailsEntity> builder)
        {
            builder.ToTable("OUTBOUND_TRANSFER_DETAILS")
                .HasKey(d => new { d.IdOutbound, d.Item });

            builder.Property(d => d.IdOutbound)
                .HasColumnName("PK_OUTBOUND")
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

            builder.HasOne(o => o.OutboundTransfer)
                .WithMany(d => d.OutboundTransferDetails)
                .HasForeignKey(d => d.IdOutbound)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(d => d.OutboundTransferDetails)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
