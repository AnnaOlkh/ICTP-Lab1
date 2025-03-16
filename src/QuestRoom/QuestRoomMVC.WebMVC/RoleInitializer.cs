using Microsoft.AspNetCore.Identity;
using QuestRoomMVC.Domain.Entities;

namespace QuestRoomMVC.WebMVC;

public class RoleInitializer
{
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string adminEmail = "admin@gmail.com";
        string adminFirstName = "admin";
        string adminLastName = "admin";
        string password = "Qwerty_1";
        if (await roleManager.FindByNameAsync("admin") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
        }
        if (await roleManager.FindByNameAsync("user") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("user"));
        }
        if (await userManager.FindByNameAsync(adminEmail) == null)
        {
            ApplicationUser admin = new ApplicationUser
            {
                Email = adminEmail, 
                FirstName = adminFirstName,
                LastName = adminLastName,
            };
            IdentityResult result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }

    }
}
