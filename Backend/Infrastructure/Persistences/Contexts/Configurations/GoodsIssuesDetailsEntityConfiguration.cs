using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsIssuesDetailsEntityConfiguration : IEntityTypeConfiguration<GoodsIssueDetailsEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsIssueDetailsEntity> builder)
        {
            builder.ToTable("GOODS_ISSUE_DETAILS")
                            .HasKey(d => new { d.IdIssue, d.Item });

            builder.Property(d => d.IdIssue)
                .HasColumnName("PK_ISSUE")
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

            builder.HasOne(g => g.GoodsIssue)
                .WithMany(d => d.GoodsIssueDetails)
                .HasForeignKey(d => d.IdIssue)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(d => d.GoodsIssueDetails)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
