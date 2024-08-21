using marketControlSpamers.Data;
using marketControlSpamers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace marketControlSpamers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeighborhoodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public NeighborhoodController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetNeighborhoods")]
        public IActionResult getNeighborhoods()
        {
            var Neighborhoods = _context.Neighborhood.Include(n => n.Township).ToList();
            return Ok(Neighborhoods);
        }

        [HttpGet]
        [Route("GetNeighborhood/{id}")]
        public IActionResult getNeighborhood(int id)
        {
            var neighborhood = _context.Neighborhood.Include(n => n.Township).FirstOrDefault(t => t.idNeighborhood == id);
            if(neighborhood == null || neighborhood.Deleted) return NotFound();
            return Ok(neighborhood);
        }

        [HttpPost]
        [Route("CreateNeighborhood")]
        public async Task<IActionResult> CreateNeighborhood([FromBody] createNeighborhood neighborhood)
        {
            if(ModelState.IsValid)
            {
                if(neighborhood.Name != null && neighborhood.Name != "" && neighborhood.Name.Length > 0 && neighborhood.idTownship > 0)
                {
                    var newNeighborhood = new Neighborhood();
                    newNeighborhood.Name = neighborhood.Name;
                    newNeighborhood.idTownship = neighborhood.idTownship;
                    if(neighborhood.PostalCode != null && neighborhood.PostalCode > 9999)
                    {
                        newNeighborhood.PostalCode = neighborhood.PostalCode;
                    }
                    newNeighborhood.Active = neighborhood.Active;
                    newNeighborhood.CreationDate = DateTime.Now;
                    newNeighborhood.CreationUser = neighborhood.CreationUser;
                    _context.Neighborhood.Add(newNeighborhood);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(getNeighborhood), new {id = newNeighborhood}, newNeighborhood);
                }
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("EditNeighborhood/{id}")]
        public IActionResult UpdateNeighborhood(long id, [FromBody] editNeighborhood neighborhood)
        {
            var existNeighborhood = _context.Neighborhood.Find(id);
            if(existNeighborhood == null || existNeighborhood.Deleted) return NotFound();
            if(neighborhood.Name != null && neighborhood.Name != "" && neighborhood.Name.Length > 0)
            {
                existNeighborhood.Name = neighborhood.Name;
            }
            if(neighborhood.idTownship > 0)
            {
                existNeighborhood.idTownship = neighborhood.idTownship;
            }
            if(neighborhood.PostalCode != null && neighborhood.PostalCode > 9999)
            {
                existNeighborhood.PostalCode = neighborhood.PostalCode;
            }
            existNeighborhood.Active = neighborhood.Active;
            existNeighborhood.EditDate = DateTime.Now;
            existNeighborhood.EditUser = neighborhood.EditUser;
            
            _context.Neighborhood.Update(existNeighborhood);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete]
        [Route("DeletNeighborhood/{id}")]
        public IActionResult DeleteNeighborhood(long id, [FromBody] int DeleteUserId)
        {
            var neighborhood = _context.Neighborhood.Find(id);
            if (neighborhood == null || neighborhood.Deleted) return NotFound();
            neighborhood.Deleted = true;
            neighborhood.DeleteDate = DateTime.Now;
            neighborhood.DeleteUser = DeleteUserId;

            _context.Neighborhood.Update(neighborhood);
            _context.SaveChanges();
            return NoContent();
        }
    }

    public class createNeighborhood
    {
        public string Name { get; set; }
        public long idTownship { get; set; }
        public int? PostalCode { get; set; }
        public bool Active { get; set; }
        public long CreationUser { get; set; }
    }

    public class editNeighborhood
    {
        public string? Name { get; set; }
        public long idTownship { get; set; }
        public int? PostalCode { get; set;}
        public bool Active { get; set; }
        public long EditUser { get; set; }
    }
}
