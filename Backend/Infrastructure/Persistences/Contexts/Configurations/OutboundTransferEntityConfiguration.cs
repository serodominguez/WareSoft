using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class OutboundTransferEntityConfiguration : IEntityTypeConfiguration<OutboundTransferEntity>
    {
        public void Configure(EntityTypeBuilder<OutboundTransferEntity> builder)
        {
            builder.ToTable("OUTBOUND_TRANSFER");

            builder.HasKey(o => o.IdOutbound);
            builder.Property(o => o.IdOutbound)
                .HasColumnName("PK_OUTBOUND");

            builder.Property(o => o.TransferCode)
                .HasColumnName("TRANSFER_CODE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(o => o.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(o => o.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(o => o.IdStoreOrigin)
                .HasColumnName("PK_STORE_ORIGIN")
                .IsRequired();

            builder.Property(o => o.IdStoreDestination)
                .HasColumnName("PK_STORE_DESTINATION")
                .IsRequired();

            builder.Property(o => o.AuditCreateUser)
              .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(o => o.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(o => o.AuditUpdateUser)
              .HasColumnName("AUDIT_UPDATE_USER");

            builder.Property(o => o.AuditUpdateDate)
                .HasColumnName("AUDIT_UPDATE_DATE");

            builder.Property(o => o.AuditDeleteUser)
              .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(o => o.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(o => o.Status)
                .HasColumnName("STATUS");

            builder.HasOne(o => o.StoreOrigin)
                .WithMany(s => s.OutboundTransfersAsOrigin)
                .HasForeignKey(o => o.IdStoreOrigin);

            builder.HasOne(o => o.StoreDestination)
                .WithMany(s => s.OutboundTransfersAsDestination)
                .HasForeignKey(o => o.IdStoreDestination);
        }
    }
}
