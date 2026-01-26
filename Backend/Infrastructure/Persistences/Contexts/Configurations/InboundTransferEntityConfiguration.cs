using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InboundTransferEntityConfiguration : IEntityTypeConfiguration<InboundTransferEntity>
    {
        public void Configure(EntityTypeBuilder<InboundTransferEntity> builder)
        {
            builder.ToTable("INBOUND_TRANSFER");

            builder.HasKey(i => i.IdInbound);
            builder.Property(i => i.IdInbound)
                .HasColumnName("PK_INBOUND");

            builder.Property(i => i.TransferCode)
                .HasColumnName("TRANSFER_CODE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(i => i.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(i => i.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(i => i.IdStoreOrigin)
                .HasColumnName("PK_STORE_ORIGIN")
                .IsRequired();

            builder.Property(i => i.IdStoreDestination)
                .HasColumnName("PK_STORE_DESTINATION")
                .IsRequired();

            builder.Property(i => i.IdOutbound)
                .HasColumnName("PK_OUTBOUND")
                .IsRequired();

            builder.Property(i => i.AuditCreateUser)
              .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(i => i.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(i => i.AuditUpdateUser)
              .HasColumnName("AUDIT_UPDATE_USER");

            builder.Property(i => i.AuditUpdateDate)
                .HasColumnName("AUDIT_UPDATE_DATE");

            builder.Property(i => i.AuditDeleteUser)
              .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(i => i.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(i => i.Status)
                .HasColumnName("STATUS");

            builder.HasOne(s => s.StoreOrigin)
                .WithMany(i => i.InboudTransfersAsOrigin)
                .HasForeignKey(i => i.IdStoreOrigin);

            builder.HasOne(s => s.StoreDestination)
                .WithMany(i => i.InboudTransfersAsDestination)
                .HasForeignKey(i => i.IdStoreDestination);

            builder.HasOne(i => i.OutboundTransfer)
                .WithOne(o => o.InboundTransfer)
                .HasForeignKey<InboundTransferEntity>(i => i.IdOutbound);
        }
    }
}
