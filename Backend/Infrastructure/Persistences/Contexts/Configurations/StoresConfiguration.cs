﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class StoresConfiguration : IEntityTypeConfiguration<Stores>
    {
        public void Configure(EntityTypeBuilder<Stores> builder)
        {
            builder.ToTable("Stores")
                .HasKey(s => s.PK_SOTRE);
            builder.Property(s => s.STORE_NAME)
                .HasMaxLength(50);
            builder.Property(s => s.MANAGER)
                .HasMaxLength(30);
            builder.Property(s => s.ADDRESS)
                .HasMaxLength(60);
            builder.Property(s => s.CITY)
                .HasMaxLength(15);
            builder.Property(s => s.EMAIL)
                .HasMaxLength(50);
            builder.Property(s => s.TYPE)
                .HasMaxLength(15);
        }
    }
}
