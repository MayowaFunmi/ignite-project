using ignite_project.Models;
using ignite_project.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ignite_project.Constants
{
  public class SeedUser
  {
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
      var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>()!;
      var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>()!;

      if (!roleManager.Roles.Any())
      {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.SuperAdmin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(UserRoles.User.ToString()));
      }

      var username = configuration.GetSection("Credentials:Username").Value;
      var password = configuration.GetSection("Credentials:Password").Value;

      if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
      {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        
        if (user == null)
        {
          var admin = new ApplicationUser
          {
            UserName = username,
            InvitationCode = GenerateCode.InvitationCode(),
            WalletAddress = "8hmfht36itgjk56890tree",
            Location = "Nigeria",
            PhoneNumber = "09045678964",
            ActiveSubscription = true,
            CreatedAt = DateTime.UtcNow
          };
          var result = await userManager.CreateAsync(admin, password);
          if (result.Succeeded)
          {
            await userManager.AddToRolesAsync(admin, [UserRoles.SuperAdmin]);
          }
          // else
          // {
          //   foreach (var error in result.Errors)
          //   {
          //     Console.WriteLine($"Code: {error.Code}, Description: {error.Description}");
          //   }
          // }
        }
      }
    }
  }
}