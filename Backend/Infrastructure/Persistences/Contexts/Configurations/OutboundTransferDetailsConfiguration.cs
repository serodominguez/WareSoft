using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class OutboundTransferDetailsConfiguration : IEntityTypeConfiguration<OutboundTransferDetails>
    {
        public void Configure(EntityTypeBuilder<OutboundTransferDetails> builder)
        {
            builder.ToTable("OutboundTransferDetails")
                .HasKey(i => new { i.PK_OUTBOUND, i.SEQUENTIAL_NUMBER });

            builder.HasOne(p => p.Products)
                .WithMany(i => i.OutboundTransferDetails)
                .HasForeignKey(i => i.PK_PRODUCT);
        }
    }
}
