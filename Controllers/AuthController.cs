using Dapper;
using Microsoft.AspNetCore.Mvc;
using StorageManagement.API.Models;
using StorageManagement.API.Repositories;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            if (await _userRepository.UsernameExistsAsync(model.Username))
                return BadRequest("Username already exists");

            if (await _userRepository.EmailExistsAsync(model.Email))
                return BadRequest("Email already exists");

            // Check if this is the first user
            bool isFirstUser = false;
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=storage.db"))
            {
                connection.Open();
                var count = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
                isFirstUser = count == 0;
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                IsAdmin = isFirstUser // First user is admin
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return Ok(new { Message = "User registered successfully", UserId = createdUser.Id, IsAdmin = createdUser.IsAdmin });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var user = await _userRepository.GetByUsernameAsync(model.Username);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return Unauthorized("Invalid username or password");

            return Ok(new { Message = "Login successful", UserId = user.Id, IsAdmin = user.IsAdmin });
        }

        [HttpPost("webtoken")]
        public async Task<IActionResult> WebToken([FromBody] LoginRequestModel model)
        {
            var user = await _userRepository.GetByUsernameAsync(model.Username);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return Unauthorized("Invalid username or password");

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Add Admin role claim if user is an admin
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}