using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestRoomMVC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Infrastracture.EntityConfigurations;

internal class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(user => user.BirthYear)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(user => user.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");
    }
}
