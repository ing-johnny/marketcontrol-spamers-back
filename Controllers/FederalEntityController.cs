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
    public class FederalEntityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FederalEntityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetFederalEntities")]
        public async Task<ActionResult<IEnumerable<FederalEntity>>> GetFederalEntities()
        {
            return await _context.FederalEntity.ToListAsync();
        }

        [HttpGet]
        [Route("GetFederalEntity/{id}")]
        public async Task<ActionResult<FederalEntity>> GetFedearlEntity(long id)
        {
            var federalEntity = await _context.FederalEntity.FindAsync(id);
            if(federalEntity == null) return NotFound();
            return federalEntity;
        }

        [HttpPost]
        [Route("CreateFederalEntity")]
        public async Task<ActionResult<FederalEntity>> CreateFederalEntity(CreatingFederalEntity federalEntity)
        {
            if(federalEntity.Name != null && federalEntity.Name != "" && federalEntity.Name.Length > 0 && federalEntity.CreationUser > 0)
            {
                var newFederalEntity = new FederalEntity();
                newFederalEntity.Name = federalEntity.Name;
                newFederalEntity.Active = federalEntity.Active;
                newFederalEntity.CreationDate = DateTime.Now;
                newFederalEntity.CreationUser = federalEntity.CreationUser;
                _context.FederalEntity.Add(newFederalEntity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetFedearlEntity), new { id = newFederalEntity.idFederalEntity}, newFederalEntity);
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("EditFederalEntity")]
        public IActionResult EditFederalEntity(long id, EditingFederalEntity federalEntity)
        {
            var newFederalEntity = _context.FederalEntity.Find(id);
            if(federalEntity.Name != null && federalEntity.Name != "" && federalEntity.Name.Length > 0)
            {
                newFederalEntity.Name = federalEntity.Name;
            }
            newFederalEntity.Active = federalEntity.Active;
            newFederalEntity.EditDate = DateTime.Now;
            newFederalEntity.EditUser = federalEntity.EditUser;
            _context.FederalEntity.Update(newFederalEntity);
            _context.SaveChanges();
            
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteFederalEntity/{id}")]
        public async Task<IActionResult> DeleteFederalEntity(long id, DeletingFederalEntity federalEntity)
        {
            var deletedFederalEntity = await _context.FederalEntity.FindAsync(id);
            if(deletedFederalEntity == null)
            {
                return NotFound();
            }
            deletedFederalEntity.DeleteUser = federalEntity.DeleteUser;
            deletedFederalEntity.EditDate = DateTime.Now;
            deletedFederalEntity.Deleted = true;

            _context.Entry(deletedFederalEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FederalEntityExists(long id)
        {
            return _context.FederalEntity.Any(e => e.idFederalEntity == id);
        }
    }

    public class CreatingFederalEntity
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public long CreationUser { get; set; }
    }

    public class EditingFederalEntity
    {
        public string? Name { get; set; }
        public bool Active { get; set; }
        public long EditUser { get; set; }
    }

    public class DeletingFederalEntity
    {
        public long DeleteUser { get; set; }
    }
}
