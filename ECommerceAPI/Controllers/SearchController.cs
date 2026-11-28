using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController (ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GlobalSearch (string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return BadRequest("Search text is required.");
            }

            var keywords = search.Trim().ToLower()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            var schoolsQuery = _context.Schools.AsQueryable();

            var classesQuery = _context.SchoolClasses.AsQueryable();

            var categoriesQuery = _context.Products.AsQueryable();

            var productsQuery = _context.Products.AsQueryable();

            foreach (var keyword in keywords)
            {
                schoolsQuery = schoolsQuery.Where(s =>
                    s.Name.ToLower().Contains(keyword));

                classesQuery = classesQuery.Where(sc =>
                    sc.Name.ToLower().Contains(keyword) ||
                    sc.School!.Name.ToLower().Contains(keyword));

                categoriesQuery = categoriesQuery.Where(p =>
                    p.Category!.Name.ToLower().Contains(keyword) ||
                    p.School!.Name.ToLower().Contains(keyword) ||
                    (p.SchoolClass != null &&
                     p.SchoolClass.Name.ToLower().Contains(keyword)));

                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword) ||
                    p.School!.Name.ToLower().Contains(keyword) ||
                    p.Category!.Name.ToLower().Contains(keyword) ||
                    (p.SchoolClass != null &&
                     p.SchoolClass.Name.ToLower().Contains(keyword)));
            }

            var schools = await schoolsQuery
                .Select(s => new
                {
                    s.Id,
                    s.Name
                })
                .ToListAsync();

            var classes = await classesQuery
                .Select(sc => new
                {
                    sc.Id,
                    sc.Name,
                    sc.SchoolId,
                    SchoolName = sc.School!.Name
                })
                .ToListAsync();

            var categories = await categoriesQuery
                .Select(p => new
                {
                    Id = p.CategoryId,
                    Name = p.Category!.Name,
                    p.SchoolId,
                    SchoolName = p.School!.Name,
                    p.SchoolClassId,
                    ClassName = p.SchoolClass != null
                        ? p.SchoolClass.Name : null
                })
                .Distinct()
                .ToListAsync();

            var products = await productsQuery
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.ImageUrl,
                    p.SchoolId,
                    SchoolName = p.School!.Name,
                    p.SchoolClassId,
                    ClassName = p.SchoolClass != null
                        ? p.SchoolClass.Name : null,
                    p.CategoryId,
                    CategoryName = p.Category!.Name
                })
                .ToListAsync();

            return Ok(new
            {
                schools,
                classes,
                categories,
                products
            });
        }
    }
}