namespace Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "owner" or "tenant"

        // KYC fields
        public string KYCStatus { get; set; } = "NotSubmitted"; // "NotSubmitted", "Pending", "Verified", "Rejected"
        public DateTime? KYCSubmittedAt { get; set; }
        public DateTime? KYCVerifiedAt { get; set; }
        public bool AadhaarVerified { get; set; } = false;
        public bool PANVerified { get; set; } = false;

        // Navigation properties
        public ICollection<KYCDocument>? KYCDocuments { get; set; }
    }
}
