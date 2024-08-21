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
    public class TownshipController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TownshipController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetTownships")]
        public IActionResult GetTownships()
        {
            var townships = _context.Township.Include(t => t.FederalEntity).ToList();
            return Ok(townships);
        }

        [HttpGet]
        [Route("GetTownship/{id}")]
        public IActionResult GetTownship(int id)
        {
            var township = _context.Township.Include(t => t.FederalEntity).FirstOrDefault(t => t.idFederalEntity == id);
            if(township == null || township.Deleted) return NotFound();
            return Ok(township);
        }

        [HttpPost]
        [Route("CreateTownship")]
        public async Task<IActionResult> CreateTownship([FromBody] createTownship township)
        {
            if(ModelState.IsValid)
            {
                if(township.Name != null && township.Name != "" && township.Name.Length > 0 && township.idFederalEntity != 0)
                {
                    var newTownship = new Township();
                    newTownship.Name = township.Name;
                    newTownship.idFederalEntity = township.idFederalEntity;
                    newTownship.Active = township.Active;
                    newTownship.CreationDate = DateTime.Now;
                    newTownship.CreationUser = township.CreationUser;
                    _context.Township.Add(newTownship);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(GetTownship), new {id = newTownship}, newTownship);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("EditTownship/{id}")]
        public IActionResult UpdateTownship(long id, [FromBody] editTownship township)
        {
            var existTownship = _context.Township.Find(id);
            if (existTownship == null || existTownship.Deleted) return NotFound();
            if(township.Name != null && township.Name != "" && township.Name.Length > 0)
            {
                existTownship.Name = township.Name;
            }
            if(township.idFederalEntity > 0)
            {
                existTownship.idFederalEntity = township.idFederalEntity;
            }
            existTownship.Active = township.Active;
            existTownship.EditDate = DateTime.Now;
            existTownship.EditUser = township.EditUser;

            _context.Township.Update(existTownship);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteTownship/{id}")]
        public IActionResult DeleteTownship(long id, [FromBody] long deleteUserId)
        {
            var township = _context.Township.Find(id);
            if (township == null || township.Deleted) return NotFound();
            township.Deleted = true;
            township.DeleteDate = DateTime.Now;
            township.DeleteUser = deleteUserId;

            _context.Township.Update(township);
            _context.SaveChanges();
            return NoContent();
        }
    }

    public class createTownship
    {
        public string Name { get; set; }
        public long idFederalEntity { get; set; }
        public bool Active { get; set; }
        public long CreationUser { get; set; }
    }

    public class editTownship
    {
        public string? Name { get; set; }
        public long idFederalEntity { get; set; }
        public bool Active { get; set; }
        public long EditUser { get; set; }
    }
}
