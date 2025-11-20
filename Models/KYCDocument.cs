namespace Server.Models
{
    public class KYCDocument
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        // Document Type: "Aadhaar" or "PAN"
        public string DocumentType { get; set; }

        // Encrypted document number
        public string DocumentNumber { get; set; }

        // For PAN: uploaded image URL
        public string? DocumentImageUrl { get; set; }

        // Verification details
        public string VerificationStatus { get; set; } = "Pending"; // "Pending", "Verified", "Failed"
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? VerifiedAt { get; set; }

        // Aadhaar-specific fields
        public string? VerifiedName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }

        // API response tracking
        public string? ApiClientId { get; set; } // For OTP flow
        public string? ApiResponse { get; set; } // Store full API response (for debugging)
    }
}
