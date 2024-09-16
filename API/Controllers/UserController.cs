using DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MODEL.DTOs;
using MODEL.Entity;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class UserController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            if(users == null)
            {
                return BadRequest("the list is empty");
            }
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user =await _context.Users.FindAsync(id);

            if (user == null)
            {
                return BadRequest("There is no user with that id");
            }
            return Ok(user);
        }

        [HttpPost("register")]  //Post: api/user/register
        //public async Task<IActionResult<User>> Register(RegistroDto registroDto)
        public async Task<IActionResult> Register(RegistroDto registroDto)
        {
            if (await UserExist(registroDto.Username))
            {
                return BadRequest("The user name is register in the aplication");
            }

            using var hmac = new HMACSHA512();
            var user = new User
            {
                UserName = registroDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDto.Password)),
                PasswordSalt = hmac.Key,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            //return user;
            return Ok(user);
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
