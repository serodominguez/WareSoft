using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
    {
        public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
        {
            builder.ToTable("GoodsReceipt")
                .HasKey(g => g.PK_RECEIPT);
            builder.Property(g => g.RECEIPT_TYPE)
                .HasMaxLength(15);
            builder.Property(g => g.NUMBER)
                .HasMaxLength(30);
            builder.Property(g => g.ANNOTATIONS)
                .HasMaxLength(80);

            builder.HasOne(s => s.Suppliers)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(g => g.PK_RECEIPT);

            builder.HasOne(s => s.Stores)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(g => g.PK_STORE);

            builder.HasOne(u => u.Users)
                .WithMany(g => g.GoodsReceipt)
                .HasForeignKey(g => g.PK_USER);
        }
    }
}
