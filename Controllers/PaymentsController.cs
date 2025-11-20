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
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Owner")
            {
                return await _context.Payments
                    .Include(p => p.Property)
                    .Where(p => p.Property.OwnerId == userId)
                    .ToListAsync();
            }
            else
            {
                return await _context.Payments
                    .Include(p => p.Property)
                    .Where(p => p.TenantId == userId)
                    .ToListAsync();
            }
        }

        [HttpPost]
        [Authorize(Roles = "tenant")]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            payment.TenantId = userId;
            payment.Date = DateTime.UtcNow;
            payment.Status = "Completed";

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayments", new { id = payment.Id }, payment);
        }
    }
}
