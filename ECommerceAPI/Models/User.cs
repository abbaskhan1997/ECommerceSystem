using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiry { get; set; }

    public List<Cart> Carts { get; set; } = new();

    public List<Order> Orders { get; set; } = new();
}