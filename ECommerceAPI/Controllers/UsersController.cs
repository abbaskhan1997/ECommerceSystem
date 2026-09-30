using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ECommerceAPI.DTOs;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController (ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("profile")]
    public IActionResult GetProfile ()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var profile = new UserProfileResponse
        {
            Name=user.Name,
            Email = user.Email,
            Role =user.Role,
        };

        return Ok(profile);
    }

    [HttpPut("profile")]
    public IActionResult UpdateProfile (UpdateProfileRequest request)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        user.Name = request.Name;
        user.Email = request.Email;

        _context.SaveChanges();

        return Ok(new UserProfileResponse
        {
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        });
    }

    [HttpPut("change-password")]
    public IActionResult ChangePassword (ChangePasswordRequest request)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(
            request.OldPassword,
            user.Password);

        if (!isPasswordCorrect)
        {
            return BadRequest("Old password is incorrect");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(
            request.NewPassword);

        _context.SaveChanges();

        return Ok("Password changed successfully");
    }
}
