using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal RentAmount { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Vacant"; // Occupied, Vacant

        // Relationships
        public int OwnerId { get; set; }
        public User? Owner { get; set; }

        public int? TenantId { get; set; }
        public User? Tenant { get; set; }
    }
}
