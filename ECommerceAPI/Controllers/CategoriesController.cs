using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController (ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCategories ()
        {
            var categories = _context.Categories.ToList();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById (int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateCategory (Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok(category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateCategory (int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            var existingCategory = _context.Categories.Find(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCategory (int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(category);
        }

    }
}
