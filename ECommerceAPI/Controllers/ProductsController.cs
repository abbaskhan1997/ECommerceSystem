using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _environment;

        public ProductsController(ApplicationDbContext context,
            IWebHostEnvironment environment )
        {
            _context = context;
            _environment = environment;
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

        [HttpPost("upload-image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadProductImage ([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("Please select an image.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Only JPG, JPEG, PNG and WEBP images are allowed.");
            }

            if (image.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Image size must not exceed 5 MB.");
            }

            var webRootPath = _environment.WebRootPath
    ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            var uploadsFolder = Path.Combine(
                webRootPath,
                "uploads",
                "products");

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/products/{fileName}";

            return Ok(new
            {
                imageUrl
            });
        }


    }
}
