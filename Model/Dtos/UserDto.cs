using System.ComponentModel.DataAnnotations;

namespace Model.Dtos;

public class UserDto
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    // Example: include role enum if needed later; for now keep minimal
    [Required]
    public string? UserName { get; set; }

    // Creation-only fields (ignored on edit)
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} must be at least {2} characters long.")]
    public string? Password { get; set; }

    // Role name (e.g., "Admin" or "Manager")
    [Required]
    [StringLength(50)]
    public string? Role { get; set; }
}
