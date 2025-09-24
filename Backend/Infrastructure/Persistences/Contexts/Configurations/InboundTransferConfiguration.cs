using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InboundTransferConfiguration : IEntityTypeConfiguration<InboundTransfer>
    {
        public void Configure(EntityTypeBuilder<InboundTransfer> builder)
        {
            builder.ToTable("InboundTransfer")
                .HasKey(i => i.PK_INBOUND);
            builder.Property(i => i.CODE)
                .HasMaxLength(15);
            builder.Property(i => i.ANNOTATIONS)
                .HasMaxLength(80);

            builder.HasOne(s => s.Stores)
                .WithMany(i => i.InboundTransfer)
                .HasForeignKey(i => i.PK_STORE);

            builder.HasOne(u => u.Users)
                .WithMany(i => i.InboundTransfer)
                .HasForeignKey(i => i.PK_USER);
        }
    }
}
