using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Services;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KYCController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AadhaarVerificationService _aadhaarService;
        private readonly ILogger<KYCController> _logger;

        public KYCController(ApplicationDbContext context, AadhaarVerificationService aadhaarService, ILogger<KYCController> logger)
        {
            _context = context;
            _aadhaarService = aadhaarService;
            _logger = logger;
        }

        // POST: api/kyc/aadhaar/generate-otp
        [HttpPost("aadhaar/generate-otp")]
        public async Task<IActionResult> GenerateAadhaarOTP([FromBody] AadhaarOTPRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("User not found");

            // Check if already verified
            if (user.AadhaarVerified)
                return BadRequest("Aadhaar already verified");

            // Generate OTP
            var response = await _aadhaarService.GenerateOTP(request.AadhaarNumber);

            if (!response.Success)
                return BadRequest(response.Message);

            // Store client ID for OTP verification
            var kycDoc = new KYCDocument
            {
                UserId = userId,
                DocumentType = "Aadhaar",
                DocumentNumber = request.AadhaarNumber, // TODO: Encrypt this
                VerificationStatus = "Pending",
                ApiClientId = response.ClientId,
                UploadedAt = DateTime.UtcNow
            };

            _context.KYCDocuments.Add(kycDoc);
            await _context.SaveChangesAsync();

            // Update user KYC status
            user.KYCStatus = "Pending";
            user.KYCSubmittedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = response.Message,
                clientId = response.ClientId,
                mockOTP = response.MockOTP // Only for development
            });
        }

        // POST: api/kyc/aadhaar/verify-otp
        [HttpPost("aadhaar/verify-otp")]
        public async Task<IActionResult> VerifyAadhaarOTP([FromBody] OTPVerifyRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound("User not found");

            // Find the KYC document with this client ID
            var kycDoc = await _context.KYCDocuments
                .Where(k => k.UserId == userId && k.ApiClientId == request.ClientId && k.DocumentType == "Aadhaar")
                .OrderByDescending(k => k.UploadedAt)
                .FirstOrDefaultAsync();

            if (kycDoc == null)
                return BadRequest("Invalid client ID or OTP request expired");

            // Verify OTP
            var response = await _aadhaarService.VerifyOTP(request.ClientId, request.OTP);

            if (!response.Success)
            {
                kycDoc.VerificationStatus = "Failed";
                await _context.SaveChangesAsync();
                return BadRequest(response.Message);
            }

            // Update KYC document with verified details
            kycDoc.VerificationStatus = "Verified";
            kycDoc.VerifiedAt = DateTime.UtcNow;
            kycDoc.VerifiedName = response.Name;
            kycDoc.DateOfBirth = response.DateOfBirth;
            kycDoc.Gender = response.Gender;
            kycDoc.Address = response.Address;
            kycDoc.ApiResponse = System.Text.Json.JsonSerializer.Serialize(response);

            // Update user status
            user.AadhaarVerified = true;
            user.KYCStatus = "Verified";
            user.KYCVerifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Aadhaar verified for user {userId}");

            return Ok(new
            {
                success = true,
                message = "Aadhaar verified successfully",
                verifiedDetails = new
                {
                    name = response.Name,
                    dob = response.DateOfBirth,
                    gender = response.Gender
                }
            });
        }

        // GET: api/kyc/status
        [HttpGet("status")]
        public async Task<IActionResult> GetKYCStatus()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users
                .Include(u => u.KYCDocuments)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            var aadhaarDoc = user.KYCDocuments?.FirstOrDefault(k => k.DocumentType == "Aadhaar" && k.VerificationStatus == "Verified");
            var panDoc = user.KYCDocuments?.FirstOrDefault(k => k.DocumentType == "PAN");

            return Ok(new
            {
                kycStatus = user.KYCStatus,
                aadhaarVerified = user.AadhaarVerified,
                panVerified = user.PANVerified,
                kycSubmittedAt = user.KYCSubmittedAt,
                kycVerifiedAt = user.KYCVerifiedAt,
                aadhaarDetails = aadhaarDoc != null ? new
                {
                    name = aadhaarDoc.VerifiedName,
                    dob = aadhaarDoc.DateOfBirth,
                    gender = aadhaarDoc.Gender,
                    lastFourDigits = aadhaarDoc.DocumentNumber?.Length >= 4 ? aadhaarDoc.DocumentNumber.Substring(aadhaarDoc.DocumentNumber.Length - 4) : ""
                } : null,
                panDetails = panDoc != null ? new
                {
                    panNumber = panDoc.DocumentNumber,
                    status = panDoc.VerificationStatus
                } : null
            });
        }

        // GET: api/kyc/documents
        [HttpGet("documents")]
        public async Task<IActionResult> GetDocuments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var documents = await _context.KYCDocuments
                .Where(k => k.UserId == userId)
                .Select(k => new
                {
                    k.Id,
                    k.DocumentType,
                    maskedNumber = k.DocumentNumber.Length >= 4 ? "********" + k.DocumentNumber.Substring(k.DocumentNumber.Length - 4) : "****",
                    k.VerificationStatus,
                    k.UploadedAt,
                    k.VerifiedAt,
                    k.VerifiedName
                })
                .ToListAsync();

            return Ok(documents);
        }
    }

    // Request models
    public class AadhaarOTPRequest
    {
        public string AadhaarNumber { get; set; } = string.Empty;
    }

    public class OTPVerifyRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }
}
