using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController (ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateOrder ()
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var cart = _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            return BadRequest("Cart is empty");
        }

        var order = new Order
        {
            UserId = userId,
            TotalAmount = cart.CartItems.Sum(
                ci => ci.Product!.Price * ci.Quantity),
            Status = "Pending"
        };

        foreach (var cartItem in cart.CartItems)
        {
            var orderItem = new OrderItem
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product!.Price
            };

            order.OrderItems.Add(orderItem);
        }

        _context.Orders.Add(order);

        _context.CartItems.RemoveRange(cart.CartItems);

        _context.SaveChanges();

        return Ok(order);
    }

    [HttpGet]
    public IActionResult GetMyOrders ()
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var orders = _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();

        return Ok(orders);
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllOrders ()
    {
        var orders = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();

        return Ok(orders);
    }

    [HttpGet("admin/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetOrderByIdForAdmin (int id)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(order);
    }

    [HttpPut("admin/{id}/status")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateOrderStatus (int id, string status)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        var allowedStatuses = new[]
        {
        "Pending",
        "Confirmed",
        "Processing",
        "Shipped",
        "Delivered",
        "Cancelled"
    };

        if (!allowedStatuses.Contains(status))
        {
            return BadRequest("Invalid order status");
        }

        order.Status = status;

        _context.SaveChanges();

        return Ok(order);
    }

    [HttpPut("admin/{id}/payment")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdatePaymentStatus (int id, string paymentStatus)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        var allowedPaymentStatuses = new[]
        {
        "Pending",
        "Paid",
        "Failed"
    };

        if (!allowedPaymentStatuses.Contains(paymentStatus))
        {
            return BadRequest("Invalid payment status");
        }

        order.PaymentStatus = paymentStatus;

        _context.SaveChanges();

        return Ok(order);
    }

    [HttpGet("{id}")]
    public IActionResult GetOrderById (int id)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var order = _context.Orders
            .Where(o => o.Id == id && o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault();

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(order);
    }

    [HttpGet("{orderId}/items/{orderItemId}")]
    public IActionResult GetOrderItemById (int orderId, int orderItemId)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var orderItem = _context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .FirstOrDefault(oi =>
                oi.Id == orderItemId &&
                oi.OrderId == orderId &&
                oi.Order!.UserId == userId);

        if (orderItem == null)
        {
            return NotFound("Order item not found");
        }

        return Ok(orderItem);
    }

    [HttpPost("{id}/checkout")]
    public IActionResult Checkout (int id)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.Id == id && o.UserId == userId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        if (order.Status != "Pending")
        {
            return BadRequest("Order is already processed");
        }

        foreach (var orderItem in order.OrderItems)
        {
            if (orderItem.Quantity > orderItem.Product!.StockQuantity)
            {
                return BadRequest(
                    $"Not enough stock for product {orderItem.ProductId}");
            }
        }

        foreach (var orderItem in order.OrderItems)
        {
            orderItem.Product!.StockQuantity -= orderItem.Quantity;
        }

        order.Status = "Confirmed";

        _context.SaveChanges();

        return Ok(order);
    }
}