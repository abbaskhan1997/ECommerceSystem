using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CartsController (ApplicationDbContext context)
        {
            _context=context;
         }

        [HttpGet]
        public IActionResult GetCart ()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var cart = _context.Carts
    .Include(c => c.CartItems)
    .ThenInclude(ci => ci.Product)
    .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpPost]
        public IActionResult AddToCart (AddToCartRequest request)
        {
            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var product = _context.Products.Find(request.ProductId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }

            if (request.Quantity > product.StockQuantity)
            {
                return BadRequest("Requested quantity is greater than available stock");
            }

            var cart = _context.Carts
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(ci =>
                    ci.CartId == cart.Id &&
                    ci.ProductId == request.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity += request.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };

                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            return Ok(cartItem);
        }

        [HttpPut("{cartItemId}")]
        public IActionResult UpdateQuantity (int cartItemId, int quantity)
        {
            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var cartItem = _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefault(ci =>
                    ci.Id == cartItemId &&
                    ci.Cart!.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }

            cartItem.Quantity = quantity;

            _context.SaveChanges();

            return Ok(cartItem);
        }

        [HttpDelete("{cartItemId}")]
        public IActionResult RemoveFromCart (int cartItemId)
        {
            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var cartItem = _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefault(ci =>
                    ci.Id == cartItemId &&
                    ci.Cart!.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            _context.CartItems.Remove(cartItem);

            _context.SaveChanges();

            return Ok("Item removed from cart");
        }
    }
}
