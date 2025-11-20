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
    public class RequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MaintenanceRequest>>> GetRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Owner")
            {
                // Get requests for properties owned by this user
                return await _context.MaintenanceRequests
                    .Include(r => r.Property)
                    .Where(r => r.Property.OwnerId == userId)
                    .ToListAsync();
            }
            else
            {
                // Get requests created by this tenant
                return await _context.MaintenanceRequests
                    .Include(r => r.Property)
                    .Where(r => r.TenantId == userId)
                    .ToListAsync();
            }
        }

        [HttpPost]
        [Authorize(Roles = "tenant")]
        public async Task<ActionResult<MaintenanceRequest>> PostRequest(MaintenanceRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            request.TenantId = userId;
            request.Date = DateTime.UtcNow;
            request.Status = "Pending";

            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequests", new { id = request.Id }, request);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] string status)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
