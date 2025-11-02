using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class UsersConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(e => e.PK_ENTITY);

            builder.Property(e => e.PK_ENTITY)
                .HasColumnName("PK_USER");

            builder.Property(u => u.USER_NAME)
                .HasMaxLength(20);
            builder.Property(u => u.NAMES)
                .HasMaxLength(30);
            builder.Property(u => u.LAST_NAMES)
                .HasMaxLength(50);
            builder.Property(u => u.IDENTIFICATION_NUMBER)
                .HasMaxLength(10);

            builder.HasOne(r => r.Roles)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.PK_ROLE);

            builder.HasOne(s => s.Stores)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.PK_STORE);
        }
    }
}
