using AuthServiceAPI.Data;
using AuthServiceAPI.Helpers;
using AuthServiceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace AuthServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly string _secretKey;
        public UserController(IConfiguration configuration, AppDBContext db_context, ILogger<AuthController> logger)
        {
            _secretKey = configuration["JWT_SECRET"];
            _context = db_context;
            _logger = logger;
        }


        private readonly int _tokenExpiryMinutes = 60;

        [HttpGet()]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserInfo()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Models.User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(currentUser == null)
            {
                return BadRequest("User doesn't exist.");
            }
            UserProfileModel userInfo = new UserProfileModel
            {
                Id = currentUser.Id,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Credential.Email,
                Birthday = (DateOnly)currentUser.Birthday
            };

            return Ok(userInfo);

        }

        [HttpPut()]
        [Authorize]
        public async Task<ActionResult> EditCurrentUserInfo([FromBody] UserProfileModel userInfo)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Models.User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            currentUser.FirstName = userInfo.FirstName ?? currentUser.FirstName;
            currentUser.LastName = userInfo.LastName ?? currentUser.LastName;
            currentUser.Credential.Email = userInfo.Email ?? currentUser.Credential.Email;
            if (userInfo.Birthday != null)
                currentUser.Birthday = userInfo.Birthday;
            

            try
            {
                await _context.SaveChangesAsync();
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "An error occurred while updating the user.");
            }


        }


        [HttpDelete()]
        [Authorize]
        public async Task<ActionResult> DeleteCurrentUser()
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId))
            {
                return Unauthorized("User authentication failed.");
            }

            Models.User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            


            try
            {
                _context.Users.Remove(currentUser);
                await _context.SaveChangesAsync();
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, "An error occurred while deleting the user.");
            }



        }
    }
}
