using marketControlSpamers.Data;
using marketControlSpamers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketControlSpamers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetRoles")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        [HttpGet]
        [Route("GetRole/{id}")]
        public async Task<ActionResult<Role>> GetRole(long id)
        {
            var role = await _context.Roles.FindAsync(id);
            if(role == null) return NotFound();
            return role;
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<ActionResult<Role>> CreateRole(creatingRole role)
        {
            if(role.Name != null && role.Name != "" && role.Name.Length > 0 && role.CreationUser > 0)
            {
                var newRole = new Role();
                newRole.Name = role.Name;
                newRole.Active = role.Active;
                newRole.CreationDate = DateTime.Now;
                newRole.CreationUser = role.CreationUser;
                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRole), new { id = newRole.idRoles }, newRole);
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("EditRole")]
        public IActionResult EditRole(long id, editingRole role)
        {
            var newRole = new Role();
            if(role.Name != null && role.Name != "" && role.Name.Length > 0)
            {
                newRole.Name = role.Name;
            }
            newRole.Active = role.Active;
            newRole.EditDate = DateTime.Now;
            newRole.EditUser = role.EditUser;
            _context.Entry(newRole).State = EntityState.Modified;
            try
            {
                _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(long id, deletingRole role)
        {
            var deletedRole = await _context.Roles.FindAsync(id);
            if(deletedRole == null)
            {
                return NotFound();
            }

            deletedRole.DeleteUser = role.DeleteUser;
            deletedRole.EditDate = DateTime.Now;
            deletedRole.Deleted = true;
            _context.Entry(deletedRole).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(long id)
        {
            return _context.Roles.Any(e => e.idRoles == id);
        }
    }

    public class creatingRole
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public long CreationUser { get; set; }
    }

    public class editingRole
    {
        public string? Name { get; set; }
        public bool Active { get; set; }
        public long EditUser { get; set; }
    }

    public class deletingRole
    {
        public long DeleteUser { get; set; }
    }
}
