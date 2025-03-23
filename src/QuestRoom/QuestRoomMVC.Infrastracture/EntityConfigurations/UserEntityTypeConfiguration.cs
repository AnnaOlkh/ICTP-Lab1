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
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).UseIdentityColumn();
        builder.HasOne(u => u.ApplicationUser)
            .WithOne(a => a.UserProfile)
            .HasForeignKey<User>(u => u.ApplicationUserId);
    }
}
