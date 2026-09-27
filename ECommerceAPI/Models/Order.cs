namespace ECommerceAPI.Models;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public List<OrderItem> OrderItems { get; set; } = new();
}