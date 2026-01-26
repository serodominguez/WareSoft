using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GoodsIssuesEntityConfiguration : IEntityTypeConfiguration<GoodsIssueEntity>
    {
        public void Configure(EntityTypeBuilder<GoodsIssueEntity> builder)
        {
            builder.ToTable("GOODS_ISSUE");

            builder.HasKey(g => g.IdIssue);
            builder.Property(g => g.IdIssue)
                .HasColumnName("PK_ISSUE");

            builder.Property(g => g.Code)
                .HasColumnName("CODE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(g => g.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(g => g.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(g => g.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(g => g.IdUser)
                .HasColumnName("PK_USER")
                .IsRequired();

            builder.Property(g => g.IdStore)
                .HasColumnName("PK_STORE")
                .IsRequired();

            builder.Property(e => e.AuditCreateUser)
                .HasColumnName("AUDIT_CREATE_USER");

            builder.Property(e => e.AuditCreateDate)
                .HasColumnName("AUDIT_CREATE_DATE");

            builder.Property(g => g.AuditDeleteUser)
              .HasColumnName("AUDIT_DELETE_USER");

            builder.Property(g => g.AuditDeleteDate)
                .HasColumnName("AUDIT_DELETE_DATE");

            builder.Property(g => g.Status)
                .HasColumnName("STATUS");

            builder.HasOne(u => u.User)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.IdUser);

            builder.HasOne(s => s.Store)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.IdStore);
        }
    }
}
