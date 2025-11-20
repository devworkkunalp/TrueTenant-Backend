namespace Server.Models
{
    public class MaintenanceRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Resolved, Rejected
        public string Type { get; set; } = "General";
        public string Priority { get; set; } = "Medium";
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Relationships
        public int PropertyId { get; set; }
        public Property? Property { get; set; }

        public int TenantId { get; set; }
        public User? Tenant { get; set; }
    }
}
