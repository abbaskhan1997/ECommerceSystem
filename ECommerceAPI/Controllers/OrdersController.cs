
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
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

    // Customer places an order using Cash on Delivery
    [HttpPost]
    [Authorize(Roles = "User")]
    public IActionResult CreateOrder (CreateOrderRequest request)
    {
        using var transaction = _context.Database.BeginTransaction(
            System.Data.IsolationLevel.Serializable);

        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var cart = _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ProductSize)
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            return BadRequest("Cart is empty");
        }

        // Check stock availability before creating the order
        foreach (var item in cart.CartItems)
        {
            if (item.Product == null)
            {
                return BadRequest("Product not found");
            }

            if (item.ProductSizeId != null && item.ProductSize == null)
            {
                return BadRequest("Selected product size not found");
            }

            var availableStock = item.ProductSize != null
                ? item.ProductSize.StockQuantity
                : item.Product.StockQuantity;

            if (item.Quantity > availableStock)
            {
                return BadRequest(
                    $"Not enough stock for product {item.ProductId}");
            }
        }

        // Create delivery address
        var deliveryAddress = new DeliveryAddress
        {
            FullName = request.FullName,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City
        };

        // Create order
        var order = new Order
        {
            UserId = userId,
            TotalAmount = cart.CartItems.Sum(
                item => item.Product!.Price * item.Quantity),
            Status = "Pending",
            PaymentMethod = "COD",
            PaymentStatus = "Pending",
            DeliveryAddress = deliveryAddress
        };

        // Create order items and reduce stock
        foreach (var item in cart.CartItems)
        {
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                ProductSizeId = item.ProductSizeId,
                Quantity = item.Quantity,
                Price = item.Product!.Price
            };

            order.OrderItems.Add(orderItem);

            if (item.ProductSize != null)
            {
                item.ProductSize.StockQuantity -= item.Quantity;
            }
            else
            {
                item.Product.StockQuantity -= item.Quantity;
            }
        }

        // Update main product stock from its sizes
        var sizedProductIds = cart.CartItems
            .Where(item => item.ProductSizeId != null)
            .Select(item => item.ProductId)
            .Distinct()
            .ToList();

        foreach (var productId in sizedProductIds)
        {
            var product = cart.CartItems
                .First(item => item.ProductId == productId).Product!;

            var sizes = _context.ProductSizes
                .Where(size => size.ProductId == productId)
                .ToList();

            product.StockQuantity = sizes.Sum(size => size.StockQuantity);
        }

        // Save order and clear cart
        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.CartItems);

        _context.SaveChanges();
        transaction.Commit();

        return Ok(order);
    }

    // Customer gets their own orders
    [HttpGet]
    [Authorize(Roles = "User")]
    public IActionResult GetMyOrders ()
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var orders = _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p!.School)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductSize)
            .ToList();

        return Ok(orders);
    }

    // Admin gets all orders
    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllOrders ()
    {
        var orders = _context.Orders
            .Include(o => o.User)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductSize)
            .ToList();

        return Ok(orders);
    }

    // Admin gets a specific order
    [HttpGet("admin/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetOrderByIdForAdmin (int id)
    {
        var order = _context.Orders
            .Include(o => o.User)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductSize)
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(order);
    }

    // Admin updates order status
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

    // Admin updates payment status
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

    // Customer gets one of their own orders
    [HttpGet("{id}")]
    [Authorize(Roles = "User")]
    public IActionResult GetOrderById (int id)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var order = _context.Orders
            .Where(o => o.Id == id && o.UserId == userId)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductSize)
            .FirstOrDefault();

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(order);
    }

    // Customer gets a specific item from their own order
    [HttpGet("{orderId}/items/{orderItemId}")]
    [Authorize(Roles = "User")]
    public IActionResult GetOrderItemById (int orderId, int orderItemId)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var orderItem = _context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.ProductSize)
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
}
