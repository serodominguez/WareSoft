using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class PermissionsConfiguration : IEntityTypeConfiguration<Permissions>
    {
        public void Configure(EntityTypeBuilder<Permissions> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(e => e.PK_ENTITY);

            builder.Property(e => e.PK_ENTITY)
                .HasColumnName("PK_PERMISSION");

            builder.HasOne(r => r.Roles)
                .WithMany(p => p.Permissions)
                .HasForeignKey(p => p.PK_ROLE);

            builder.HasOne(m => m.Modules)
                .WithMany(p => p.Permissions)
                .HasForeignKey(m => m.PK_MODULE);

            builder.HasOne(a => a.Actions)
                .WithMany(p => p.Permissions)
                .HasForeignKey(a => a.PK_ACTION);


            builder.HasIndex(p => new { p.PK_ROLE, p.PK_MODULE, p.PK_ACTION })
                .IsUnique()
                .HasDatabaseName("IX_Permissions_Role_Module_Action");

            builder.HasIndex(p => p.PK_ROLE)
                .HasDatabaseName("IX_Permissions_Role");

            builder.HasIndex(p => p.STATE)
                .HasDatabaseName("IX_Permissions_State");
        }
    }
}
