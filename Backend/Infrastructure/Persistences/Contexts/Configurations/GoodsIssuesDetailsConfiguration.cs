using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsIssuesDetailsConfiguration : IEntityTypeConfiguration<GoodsIssueDetails>
    {
        public void Configure(EntityTypeBuilder<GoodsIssueDetails> builder)
        {
            builder.ToTable("GoodsIssueDetails")
                .HasKey(g => new { g.PK_ISSUE, g.SEQUENTIAL_NUMBER });

            builder.HasOne(p => p.Products)
                .WithMany(i => i.GoodsIssueDetails)
                .HasForeignKey(i => i.PK_PRODUCTO);
        }
    }
}
