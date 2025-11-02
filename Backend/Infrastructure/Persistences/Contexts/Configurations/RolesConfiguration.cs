using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class RolesConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(e => e.PK_ENTITY);

            builder.Property(e => e.PK_ENTITY)
                .HasColumnName("PK_ROLE");

            builder.Property(r => r.ROLE_NAME)
                .HasMaxLength(20);
        }
    }
}
