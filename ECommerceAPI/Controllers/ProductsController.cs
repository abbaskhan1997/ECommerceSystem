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

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchWords = search
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in searchWords)
                {
                    products = products.Where(p =>
                        (p.Name != null && p.Name.ToLower().Contains(word)) ||
                        (p.School != null && p.School.Name.ToLower().Contains(word)) ||
                        (p.SchoolClass != null && p.SchoolClass.Name.ToLower().Contains(word))
                    );
                }
            }

            // Filter products by category
            if (categoryId.HasValue)
            {
                products = products.Where(p =>
                    p.CategoryId == categoryId.Value);
            }

            // Filter products by school
            if (schoolId.HasValue)
            {
                products = products.Where(p => p.SchoolId == schoolId.Value);
            }

            // Filter products by school class
            if (schoolClassId.HasValue)
            {
                products = products.Where(p => p.SchoolClassId == schoolClassId.Value);
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
