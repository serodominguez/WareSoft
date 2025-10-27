using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ModulesConfiguration : IEntityTypeConfiguration<Modules>
    {
        public void Configure(EntityTypeBuilder<Modules> builder)
        {
            builder.ToTable("Modules")
                .HasKey(m => m.PK_MODULE);
            builder.Property(m => m.MODULE_NAME)
                .HasMaxLength(25);
        }
    }
}
