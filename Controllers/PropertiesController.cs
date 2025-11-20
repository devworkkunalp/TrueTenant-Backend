using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
        {
            return await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Tenant)
                .ToListAsync();
        }

        [HttpGet("owner")]
        [Authorize(Roles = "owner")]
        public async Task<ActionResult<IEnumerable<Property>>> GetMyProperties()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return await _context.Properties
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Tenant)
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "owner")]
        public async Task<ActionResult<Property>> PostProperty(Property property)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            property.OwnerId = userId;
            
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProperties", new { id = property.Id }, property);
        }
        
        // PUT: api/Properties/5
        [HttpPut("{id}")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> PutProperty(int id, Property property)
        {
            if (id != property.Id)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingProperty = await _context.Properties.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (existingProperty == null)
            {
                return NotFound();
            }

            if (existingProperty.OwnerId != userId)
            {
                return Forbid();
            }

            property.OwnerId = userId; // Ensure owner doesn't change
            _context.Entry(property).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyExists(id))
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

        // DELETE: api/Properties/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            if (property.OwnerId != userId)
            {
                return Forbid();
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}
