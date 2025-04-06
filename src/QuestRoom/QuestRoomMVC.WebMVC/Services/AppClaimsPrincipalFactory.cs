using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using QuestRoomMVC.Domain.Entities;
using System.Security.Claims;

namespace QuestRoomMVC.WebMVC.Services;

public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public AppClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Гарантуємо, що додається ApplicationUser.Id як NameIdentifier
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

        return identity;
    }
}
