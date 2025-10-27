using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class PermissionsConfiguration : IEntityTypeConfiguration<Permissions>
    {
        public void Configure(EntityTypeBuilder<Permissions> builder)
        {
            builder.ToTable("Permissions")
                .HasKey(a => a.PK_PERMISSIONS);

            builder.HasOne(r => r.Roles)
                .WithMany(p => p.Permissions)
                .HasForeignKey(p => p.PK_ROLE);

            builder.HasOne(m => m.Modules)
                .WithMany(p => p.Permissions)
                .HasForeignKey(m => m.PK_MODULE);

            builder.HasOne(a => a.Actions)
                .WithMany(p => p.Permissions)
                .HasForeignKey(a => a.PK_ACTION);

        }
    }
}
