using AuthServiceAPI.Data;
using AuthServiceAPI.Helpers;
using AuthServiceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AuthServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly string _secretKey;
        public AuthController(IConfiguration configuration, AppDBContext db_context, ILogger<AuthController> logger)
        {
            _secretKey = configuration["JWT_SECRET"];
            _context = db_context;
            _logger = logger;
        }

        
        private readonly int _tokenExpiryMinutes = 60;

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] UserCredential userCredentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Credential.Email == userCredentials.Email);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            else if(user.Credential.Password.Equals(userCredentials.Password)) { }
            {
                // login successful
                var token = JwtHelper.GenerateToken(user.Credential.Email, user.Id.ToString(), _secretKey, _tokenExpiryMinutes);
                return Ok(new TokenResponse
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(_tokenExpiryMinutes)
                });

            }
            

        }


        [HttpPost("register")]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] UserCreationModel userCredentials)
        {


            if (EmailHelper.IsValidEmail(userCredentials.Email) && !string.IsNullOrEmpty(userCredentials.Password) && !string.IsNullOrEmpty(userCredentials.FirstName) && !string.IsNullOrEmpty(userCredentials.LastName))
            {
                _context.Users.Add(new Models.User
                {
                    Credential = new UserCredential
                    {
                        Email = userCredentials.Email,
                        Password = userCredentials.Password
                    },
                    FirstName = userCredentials.FirstName,
                    LastName = userCredentials.LastName,
                    Birthday = userCredentials.Birthday,
                    
                });
                await _context.SaveChangesAsync();
                return Ok("User was created succesfully."); 

            }

            return BadRequest("Invalid user data.");

        }
    }
}
