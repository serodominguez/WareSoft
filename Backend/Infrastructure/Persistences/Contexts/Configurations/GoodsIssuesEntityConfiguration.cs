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

            builder.Property(g => g.CreateDate)
                .HasColumnName("CREATE_DATE")
                .IsRequired();

            builder.Property(g => g.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(g => g.TotalAmount)
                .HasColumnName("TOTAL_AMOUNT")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(g => g.Annotations)
                .HasColumnName("ANNOTATIONS")
                .HasMaxLength(80);

            builder.Property(g => g.IdCustomer)
                .HasColumnName("PK_CUSTOMER")
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

            builder.HasOne(c => c.Customer)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.IdCustomer);

            builder.HasOne(s => s.Store)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.IdStore);

            builder.HasOne(u => u.User)
                .WithMany(g => g.GoodsIssue)
                .HasForeignKey(g => g.IdUser);
        }
    }
}
