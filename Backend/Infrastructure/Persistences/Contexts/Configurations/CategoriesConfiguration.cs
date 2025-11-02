using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class CategoriesConfiguration : IEntityTypeConfiguration<Categories>
    {
        public void Configure(EntityTypeBuilder<Categories> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(e => e.PK_ENTITY);

            builder.Property(e => e.PK_ENTITY)
                .HasColumnName("PK_CATEGORY");

            builder.Property(c => c.CATEGORY_NAME)
                .HasMaxLength(25);
            builder.Property(c => c.DESCRIPTION)
                .HasMaxLength(50);
        }
    }
}
