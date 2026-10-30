
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models;

public class ProductSize
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    public string Size { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
}
