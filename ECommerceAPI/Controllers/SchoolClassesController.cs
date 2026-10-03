using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolClassesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SchoolClassesController (ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetClasses ()
    {
        var classes = _context.SchoolClasses.ToList();

        return Ok(classes);
    }

    [HttpGet("{id}")]
    public IActionResult GetClassById (int id)
    {
        var schoolClass = _context.SchoolClasses.Find(id);

        if (schoolClass == null)
        {
            return NotFound("Class not found");
        }

        return Ok(schoolClass);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateClass (SchoolClass schoolClass)
    {
        _context.SchoolClasses.Add(schoolClass);
        _context.SaveChanges();

        return Ok(schoolClass);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateClass (int id, SchoolClass schoolClass)
    {
        if (id != schoolClass.Id)
        {
            return BadRequest();
        }

        _context.Entry(schoolClass).State =
            Microsoft.EntityFrameworkCore.EntityState.Modified;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteClass (int id)
    {
        var schoolClass = _context.SchoolClasses.Find(id);

        if (schoolClass == null)
        {
            return NotFound("Class not found");
        }

        _context.SchoolClasses.Remove(schoolClass);
        _context.SaveChanges();

        return Ok("Class deleted successfully");
    }
}