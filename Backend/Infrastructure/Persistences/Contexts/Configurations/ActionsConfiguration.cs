using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ActionsConfiguration : IEntityTypeConfiguration<Actions>
    {
        public void Configure(EntityTypeBuilder<Actions> builder)
        {
            builder.ToTable("Actions")
                .HasKey(a => a.PK_ACTION);
            builder.Property(a => a.ACTION_NAME)
                .HasMaxLength(10);
        }
    }
}
