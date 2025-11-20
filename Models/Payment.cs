using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Completed";

        // Relationships
        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        public int TenantId { get; set; }
        public User? Tenant { get; set; }
    }
}
