using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public string Register (ECommerceAPI.DTOs.RegisterRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return "User registered successfully";
        }

        public string Login (string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return "";
            }

            if(!BCrypt.Net.BCrypt.Verify(password, user.Password)) 
            { 
                return ""; 
            }

            var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Name),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role)
};

            var key = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
    issuer: _configuration["Jwt:Issuer"],
    audience: _configuration["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: credentials
);

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
