using DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
