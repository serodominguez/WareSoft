using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsReceiptDetailsConfiguration : IEntityTypeConfiguration<GoodsReceiptDetails>
    {
        public void Configure(EntityTypeBuilder<GoodsReceiptDetails> builder)
        {
            builder.ToTable("GoodsReceiptDetails")
                .HasKey(g => new { g.PK_RECEIPT, g.SEQUENTIAL_NUMBER });

            builder.HasOne(p => p.Products)
                .WithMany(i => i.GoodsReceiptDetails)
                .HasForeignKey(i => i.PK_PRODUCTO);
        }
    }
}
