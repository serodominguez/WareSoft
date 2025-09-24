using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class InboundTransferDetailsConfiguration : IEntityTypeConfiguration<InboundTransferDetails>
    {
        public void Configure(EntityTypeBuilder<InboundTransferDetails> builder)
        {
            builder.ToTable("InboundTransferDetails")
                .HasKey(i => new { i.PK_INBOUND, i.SEQUENTIAL_NUMBER });

            builder.HasOne(p => p.Products)
                .WithMany(i => i.InboundTransferDetails)
                .HasForeignKey(i => i.PK_PRODUCT);
        }
    }
}
