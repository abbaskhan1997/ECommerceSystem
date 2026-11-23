using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    [JsonIgnore]
    public string? ResetToken { get; set; }

    [JsonIgnore]
    public DateTime? ResetTokenExpiry { get; set; }

    public List<Cart> Carts { get; set; } = new();

    [JsonIgnore]
    public List<Order> Orders { get; set; } = new();
}