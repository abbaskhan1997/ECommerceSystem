using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SchoolsController (ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetSchools ()
    {
        var schools = _context.Schools.ToList();

        return Ok(schools);
    }

    [HttpGet("{id}")]
    public IActionResult GetSchoolById (int id)
    {
        var school = _context.Schools.Find(id);

        if (school == null)
        {
            return NotFound("School not found");
        }

        return Ok(school);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateSchool (School school)
    {
        _context.Schools.Add(school);
        _context.SaveChanges();

        return Ok(school);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateSchool (int id, School school)
    {
        if (id != school.Id)
        {
            return BadRequest();
        }

        _context.Entry(school).State =
            Microsoft.EntityFrameworkCore.EntityState.Modified;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteSchool (int id)
    {
        var school = _context.Schools.Find(id);

        if (school == null)
        {
            return NotFound("School not found");
        }

        _context.Schools.Remove(school);
        _context.SaveChanges();

        return Ok("School deleted successfully");
    }
}