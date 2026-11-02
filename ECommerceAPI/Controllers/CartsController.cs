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
       .Include(c => c.CartItems)
           .ThenInclude(ci => ci.ProductSize)
       .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound();
            }

            var totalAmount = cart.CartItems.Sum(
              ci => ci.Product!.Price * ci.Quantity);

            return Ok(new
            {
                cart.Id,
                cart.UserId,
                cart.CartItems,
                totalAmount
            });
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

            // Check whether the product has sizes
            var hasSizes = _context.ProductSizes
                .Any(ps => ps.ProductId == request.ProductId);

            ProductSize? productSize = null;

            if (hasSizes && request.ProductSizeId == null)
            {
                return BadRequest("Please select a product size");
            }

            if (!hasSizes && request.ProductSizeId != null)
            {
                return BadRequest("This product does not have sizes");
            }

            // Find the selected size
            if (request.ProductSizeId != null)
            {
                productSize = _context.ProductSizes.FirstOrDefault(ps =>
                    ps.Id == request.ProductSizeId &&
                    ps.ProductId == request.ProductId);

                if (productSize == null)
                {
                    return BadRequest("Invalid product size");
                }
            }

            // Get available stock
            var availableStock = productSize != null
                ? productSize.StockQuantity
                : product.StockQuantity;

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

            // Find cart item using product and selected size
            var cartItem = _context.CartItems.FirstOrDefault(ci =>
                ci.CartId == cart.Id &&
                ci.ProductId == request.ProductId &&
                ci.ProductSizeId == request.ProductSizeId);

            var newQuantity = request.Quantity;

            if (cartItem != null)
            {
                newQuantity += cartItem.Quantity;
            }

            if (newQuantity > availableStock)
            {
                return BadRequest("Requested quantity is greater than available stock");
            }

            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    ProductSizeId = request.ProductSizeId,
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
                .Include(ci => ci.Product)
                .Include(ci => ci.ProductSize)
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

            // Get stock from selected size or main product
            var availableStock = cartItem.ProductSizeId != null
                ? cartItem.ProductSize!.StockQuantity
                : cartItem.Product!.StockQuantity;

            if (quantity > availableStock)
            {
                return BadRequest("Requested quantity is greater than available stock");
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
