using System.Text.Json.Serialization;
namespace ECommerceAPI.Models;

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [JsonIgnore]
    public Cart? Cart { get; set; }

    public Product? Product { get; set; }
}
