using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ProductsConfiguration : IEntityTypeConfiguration<Products>
    {
        public void Configure(EntityTypeBuilder<Products> builder)
        {
            builder.ToTable("Products")
                .HasKey(p => p.PK_PRODUCT);
            builder.Property(p => p.CODE)
                .HasMaxLength(25);
            builder.Property(p => p.DESCRIPTION)
                .HasMaxLength(50);
            builder.Property(p => p.MATERIAL)
                .HasMaxLength(25);
            builder.Property(p => p.COLOR)
                .HasMaxLength(20);
            builder.Property(p => p.MEASUREMENT)
                .HasMaxLength(15);

            builder.HasOne(b => b.Brands)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.PK_BRAND);

            builder.HasOne(c => c.Categories)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.PK_CATEGORY);
        }
    }
}
