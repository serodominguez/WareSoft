using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsIssuesConfiguration : IEntityTypeConfiguration<GoodsIssue>
    {
        public void Configure(EntityTypeBuilder<GoodsIssue> builder)
        {
            builder.ToTable("GoodsIssues")
                .HasKey(g => g.PK_ISSUE);
            builder.Property(g => g.CODE)
                .HasMaxLength(15);
            builder.Property(g => g.TYPE)
                .HasMaxLength(15);
            builder.Property(g => g.ANNOTATIONS)
                .HasMaxLength(80);

            builder.HasOne(c => c.Consumers)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.PK_CONSUMER);

            builder.HasOne(s => s.Stores)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.PK_STORE);

            builder.HasOne(u => u.Users)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.PK_USER);
        }
    }
}
