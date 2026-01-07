using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFirstApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly TodoDb _db;
        private readonly IConfiguration _config;
        public AuthController(TodoDb db, IConfiguration config) { _db = db; _config = config; }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto req) {
            if (await _db.Users.AnyAsync(u => u.Username == req.Username)) return BadRequest(new { message = "User exists" });
            var user = new User { Username = req.Username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password) };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Registered" });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserDto req) {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return BadRequest(new { message = "Invalid login" });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(claims: new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }, expires: DateTime.Now.AddDays(1), signingCredentials: creds);
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
    public class UserDto { public string Username { get; set; } = ""; public string Password { get; set; } = ""; }
}