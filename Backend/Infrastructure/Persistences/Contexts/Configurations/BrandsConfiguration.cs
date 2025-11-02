using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class BrandsConfiguration : IEntityTypeConfiguration<Brands>
    {
        public void Configure(EntityTypeBuilder<Brands> builder)
        {
            builder.ToTable("Brands");

            builder.HasKey(e => e.PK_ENTITY);

            builder.Property(e => e.PK_ENTITY)
                .HasColumnName("PK_BRAND");

            builder.Property(b => b.BRAND_NAME)
                .HasMaxLength(25);
        }
    }
}
