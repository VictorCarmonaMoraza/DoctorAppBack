using DATA;
using DATA.Interfaces;
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
        private readonly ITokenService _tokenService;

        public UserController(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
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

            var userDto = new UserDto()
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            //return user;
            return Ok(userDto);
        }

        [HttpPost("login")]   //Post: api/user/login
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) {
                return Unauthorized("Invalid user"); //Return 401
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //Comparamos caracter por caracter
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password"); //Return 401
                }
            }

            var userDto = new UserDto()
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            return Ok(userDto);
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
