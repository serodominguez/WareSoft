using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsReceiptEntityConfiguration : IEntityTypeConfiguration<GoodsReceiptEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsReceiptEntity> builder)
        {
            builder.ToTable("GOODS_RECEIPT");

            builder.HasKey(g => g.IdReceipt);
            builder.Property(g => g.IdReceipt)
                .HasColumnName("PK_RECEIPT");

            builder.Property(g => g.Code)
                .HasColumnName("CODE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(g => g.PurchaseDate)
                .HasColumnName("PURCHASE_DATE")
                .IsRequired();

            builder.Property(g => g.CreateDate)
                .HasColumnName("CREATE_DATE")
                .IsRequired();

            builder.Property(g => g.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(g => g.Number)
                .HasColumnName("NUMBER")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(g => g.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(g => g.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(g => g.IdSupplier)
                .HasColumnName("PK_SUPPLIER")
                .IsRequired();

            builder.Property(g => g.IdStore)
                .HasColumnName("PK_STORE")
                .IsRequired();

            builder.Property(g => g.IdUser)
                .HasColumnName("PK_USER")
                .IsRequired();

            builder.Property(g => g.AuditDeleteUser)
              .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(g => g.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(g => g.Status)
                .HasColumnName("STATUS");

            builder.HasOne(s => s.Supplier)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(g => g.IdSupplier);

            builder.HasOne(s => s.Store)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(g => g.IdStore);

            builder.HasOne(u => u.User)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(g => g.IdUser);
        }
    }
}
