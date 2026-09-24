using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using ECommerceAPI.DTOs;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController (AuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public IActionResult Register (ECommerceAPI.DTOs.RegisterRequest request)
        {
            var result = _authService.Register(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login (string email, string password)
        {
            var token = _authService.Login(email, password);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new { token });
        }
    }
}
