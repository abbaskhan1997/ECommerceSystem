using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts (
            string? search,
             int? categoryId,
             int? schoolId,
             int? schoolClassId,
             int pageNumber = 1,
             int pageSize = 10)
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Category)
                .Include(p => p.School)
                .Include(p => p.SchoolClass)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p =>
                    p.CategoryId == categoryId.Value);
            }

            var totalProducts = products.Count();
            var skip =(pageNumber - 1) * pageSize;

            var result = products.Skip(skip)
                .Take(pageSize)
                .ToList();


            return Ok(new
            {
                totalProducts,
                pageNumber,
                pageSize,
                products = result
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById (int id)
        {
            var product = _context.Products
     .Include(p => p.Category)
     .Include(p => p.School)
     .Include(p => p.SchoolClass)
     .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }


    }
}
