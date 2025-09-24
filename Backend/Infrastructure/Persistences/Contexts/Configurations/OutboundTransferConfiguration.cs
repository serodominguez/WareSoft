using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class OutboundTransferConfiguration : IEntityTypeConfiguration<OutboundTransfer>
    {
        public void Configure(EntityTypeBuilder<OutboundTransfer> builder)
        {
            builder.ToTable("OutboundTransfer")
                .HasKey(o => o.PK_OUTBOUND);
            builder.Property(o => o.CODE)
                .HasMaxLength(15);
            builder.Property(o => o.ANNOTATIONS)
                .HasMaxLength(80);

            builder.HasOne(s => s.Stores)
                .WithMany(i => i.OutboundTransfer)
                .HasForeignKey(i => i.PK_STORE);

            builder.HasOne(u => u.Users)
                .WithMany(i => i.OutboundTransfer)
                .HasForeignKey(i => i.PK_USER);
        }
    }
}
