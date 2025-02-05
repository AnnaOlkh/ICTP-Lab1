using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuestRoomMVC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace QuestRoomMVC.Infrastracture.EntityConfigurations;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> 
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //builder.ToTable("Users");
        builder.HasKey(user => user.ID);
        builder.Property(user => user.ID).UseIdentityColumn();

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(user => user.Role)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt);
    }
}
