using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class SuppliersConfiguration : IEntityTypeConfiguration<Suppliers>
    {
        public void Configure(EntityTypeBuilder<Suppliers> builder)
        {
            builder.ToTable("Suppliers")
                .HasKey(s => s.PK_SUPPLIER);
            builder.Property(s => s.COMPANY_NAME)
                .HasMaxLength(50);
            builder.Property(s => s.CONTACT)
                .HasMaxLength(30);
            builder.Property(s => s.EMAIL)
                .HasMaxLength(50);
        }
    }
}
