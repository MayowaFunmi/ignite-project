using ignite_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ignite_project.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
  {

    public DbSet<SignUpCode> SignUpCodes { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<IdentityUserRole<string>>().HasKey(ur => new { ur.UserId, ur.RoleId });

      // builder.Entity<ApplicationUser>()
      //   .HasIndex(u => u.WalletAddress)
      //   .IsUnique();

      builder.Entity<SignUpCode>()
        .HasIndex(u => u.Code)
        .IsUnique();
    }
  }
}