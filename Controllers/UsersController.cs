using marketControlSpamers.Data;
using marketControlSpamers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketControlSpamers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.Include(u => u.Role).ToList();
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUser/{id}")]
        public IActionResult GetUser(long id)
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.IdUsers == id);
            if(user == null || user.Deleted) return NotFound();
            return Ok(user);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] createUser user)
        {
            if(ModelState.IsValid)
            {
                if(user.Name != null && user.Name != "" && user.Name.Length != 0 && user.Password != null && user.Password != "" && user.Password.Length != 0 && user.idRoles != null
                    && user.idRoles != 0 && user.Active != null && user.CreationUser != null && user.CreationUser != 0)
                {
                    var newUser = new User();
                    newUser.Name = user.Name;
                    newUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    newUser.idRoles = user.idRoles;
                    newUser.Active = user.Active;
                    newUser.CreationDate = DateTime.Now;
                    newUser.CreationUser = user.CreationUser;
                    newUser.EditDate = null;
                    newUser.EditUser = 0;
                    newUser.DeleteDate = null;
                    newUser.DeleteUser = 0;
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(GetUser), new { id = newUser.IdUsers }, newUser);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("EditUser/{id}")]
        public IActionResult updateUser(long id, [FromBody] ediUser user)
        {
            var existingUser = _context.Users.Find(id);
            if(existingUser == null || existingUser.Deleted) return NotFound();

            if(user.Name != null && user.Name != "" && user.Name.Length != 0)
            {
                existingUser.Name = user.Name;
            }
            if(user.Password != null && user.Password != "" && user.Password.Length != 0)
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword((string)user.Password);
            }
            if(user.idRoles != null && user.idRoles != 0)
            {
                existingUser.idRoles = user.idRoles;
            }
            existingUser.Active = user.Active;
            existingUser.EditDate = DateTime.Now;
            existingUser.EditUser = user.EditUser;

            _context.Users.Update(existingUser);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public IActionResult deleteUser(long id, [FromBody] long deleteUserId)
        {
            var user = _context.Users.Find(id);
            if(user == null || user.Deleted) return NotFound();

            user.Deleted = true;
            user.DeleteDate = DateTime.Now;
            user.DeleteUser = deleteUserId;

            _context.Users.Update(user);
            _context.SaveChanges();
            return NoContent();
        }
    }

    public class ediUser
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public long idRoles { get; set; }
        public bool Active { get; set; }
        public long EditUser { get; set; }
    }

    public class createUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public long idRoles { get; set; }
        public bool Active { get; set; }
        public long CreationUser { get; set; }
    }
}
