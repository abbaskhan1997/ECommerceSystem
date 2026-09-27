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

    [HttpGet("{id}")]
    public IActionResult GetOrderById (int id)
    {
        var userId = int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var order = _context.Orders
            .Where(o => o.UserId == userId && o.Id == id)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault();

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
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
}