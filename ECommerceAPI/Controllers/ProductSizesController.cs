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

        public ProductSizesController (ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{productId}")]
        public IActionResult GetProductSizes (int productId)
        {
            var sizes = _context.ProductSizes
                .Where(ps => ps.ProductId == productId)
                .ToList();

            return Ok(sizes);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProductSize (ProductSize productSize)
        {
            _context.ProductSizes.Add(productSize);
            _context.SaveChanges();

            return Ok(productSize);
        }

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

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProductSize (int id)
        {
            var productSize = _context.ProductSizes.Find(id);

            if (productSize == null)
                return NotFound();

            _context.ProductSizes.Remove(productSize);
            _context.SaveChanges();

            return NoContent();
        }
    }
}