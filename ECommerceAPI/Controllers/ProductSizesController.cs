
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductSizesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection: ApplicationDbContext is injected through the constructor.
        public ProductSizesController (ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all sizes of a specific product.
        [HttpGet("{productId}")]
        public IActionResult GetProductSizes (int productId)
        {
            var sizes = _context.ProductSizes
                .Where(ps => ps.ProductId == productId)
                .ToList();

            return Ok(sizes);
        }

        // Add a new product size.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProductSize (ProductSize productSize)
        {
            var product = _context.Products.Find(productSize.ProductId);

            if (product == null)
                return NotFound("Product not found.");

            _context.ProductSizes.Add(productSize);
            _context.SaveChanges();

            // Recalculate the main stock after adding a size.
            UpdateMainStock(productSize.ProductId);

            return Ok(productSize);
        }

        // Update an existing product size.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProductSize (int id, ProductSize updatedSize)
        {
            if (id != updatedSize.Id)
                return BadRequest();

            var existingSize = _context.ProductSizes.Find(id);

            if (existingSize == null)
                return NotFound();

            existingSize.Size = updatedSize.Size;
            existingSize.StockQuantity = updatedSize.StockQuantity;

            _context.SaveChanges();

            // Recalculate the main stock after updating a size.
            UpdateMainStock(existingSize.ProductId);

            return NoContent();
        }

        // Delete an existing product size.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProductSize (int id)
        {
            var productSize = _context.ProductSizes.Find(id);

            if (productSize == null)
                return NotFound();

            int productId = productSize.ProductId;

            _context.ProductSizes.Remove(productSize);
            _context.SaveChanges();

            // Recalculate the main stock after deleting a size.
            UpdateMainStock(productId);

            return NoContent();
        }

        // Calculate the total quantity of all sizes and update the main product stock.
        private void UpdateMainStock (int productId)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
                return;

            int totalStock = _context.ProductSizes
                .Where(ps => ps.ProductId == productId)
                .Sum(ps => (int?)ps.StockQuantity) ?? 0;

            product.StockQuantity = totalStock;

            _context.SaveChanges();
        }
    }
}
