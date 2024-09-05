using ignite_project.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ignite_project.Configurations
{
  public class AuthPolicy
  {
    public static void ConfigureAuthorization(AuthorizationOptions options)
    {
      options.AddPolicy("SuperAdmin", policy => policy.RequireRole(UserRoles.SuperAdmin.ToString()));
      options.AddPolicy("Admin", policy => policy.RequireRole(UserRoles.Admin.ToString()));
      options.AddPolicy("User", policy => policy.RequireRole(UserRoles.User.ToString()));
      options.AddPolicy("AdminSuperAdmin", policy => policy.RequireRole(UserRoles.SuperAdmin, UserRoles.Admin.ToString()));
    }
  }
}