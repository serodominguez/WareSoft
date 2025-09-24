using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ConsumersConfiguration : IEntityTypeConfiguration<Consumers>
    {
        public void Configure(EntityTypeBuilder<Consumers> builder)
        {
            builder.ToTable("Consumers")
                .HasKey(c => c.PK_CONSUMER);
            builder.Property(c => c.NAMES)
                .HasMaxLength(30);
            builder.Property(c => c.LAST_NAMES)
                .HasMaxLength(50);
            builder.Property(c => c.IDENTIFICATION_NUMBER)
                .HasMaxLength(10);
        }
    }
}
