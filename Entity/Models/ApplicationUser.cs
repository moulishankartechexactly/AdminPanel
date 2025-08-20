using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entity.Models
{
    public enum UserRole
    {
        Manager = 0,
        Admin = 1
    }

    // Extends IdentityUser to allow custom profile fields.
    public class ApplicationUser : IdentityUser
    {
        // Optional: persist a primary role choice alongside standard ASP.NET Core Identity roles.
        // If you prefer to rely solely on ASP.NET Core roles tables/claims, you can remove this.
        [Required]
        public UserRole Role { get; set; } = UserRole.Manager;

        // Example of custom profile fields you may add later
        // public string? FullName { get; set; }
    }
}
