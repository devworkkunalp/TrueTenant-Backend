using Server.Models;

namespace Server.Services
{
    public class AadhaarVerificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AadhaarVerificationService> _logger;

        public AadhaarVerificationService(IConfiguration configuration, ILogger<AadhaarVerificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Generate OTP for Aadhaar verification
        public async Task<AadhaarOTPResponse> GenerateOTP(string aadhaarNumber)
        {
            // Validate Aadhaar number
            if (string.IsNullOrWhiteSpace(aadhaarNumber) || aadhaarNumber.Length != 12 || !aadhaarNumber.All(char.IsDigit))
            {
                return new AadhaarOTPResponse { Success = false, Message = "Invalid Aadhaar number" };
            }

            // TODO: Replace with actual API call when credentials are available
            // For now, using mock implementation
            var apiEnabled = _configuration.GetValue<bool>("AadhaarAPI:Enabled", false);

            if (apiEnabled)
            {
                // Real API implementation would go here
                // var apiKey = _configuration["AadhaarAPI:ApiKey"];
                // var baseUrl = _configuration["AadhaarAPI:BaseUrl"];
                // Make HTTP call to Aadhaar API
            }

            // Mock implementation for development
            _logger.LogInformation($"Mock: Generating OTP for Aadhaar: {MaskAadhaar(aadhaarNumber)}");
            
            var clientId = Guid.NewGuid().ToString();
            
            return await Task.FromResult(new AadhaarOTPResponse
            {
                Success = true,
                ClientId = clientId,
                Message = "OTP sent to registered mobile number",
                MockOTP = "123456" // Only for development! Remove in production
            });
        }

        // Verify OTP and get Aadhaar details
        public async Task<AadhaarVerifyResponse> VerifyOTP(string clientId, string otp)
        {
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(otp))
            {
                return new AadhaarVerifyResponse { Success = false, Message = "Invalid client ID or OTP" };
            }

            // TODO: Replace with actual API call
            var apiEnabled = _configuration.GetValue<bool>("AadhaarAPI:Enabled", false);

            if (apiEnabled)
            {
                // Real API implementation would go here
            }

            // Mock implementation for development
            _logger.LogInformation($"Mock: Verifying OTP for client: {clientId}");

            // Simulate OTP verification (accept "123456" for development)
            if (otp != "123456")
            {
                return new AadhaarVerifyResponse { Success = false, Message = "Invalid OTP" };
            }

            // Return mock user details
            return await Task.FromResult(new AadhaarVerifyResponse
            {
                Success = true,
                Message = "Aadhaar verified successfully",
                Name = "Test User",
                DateOfBirth = "01-01-1990",
                Gender = "M",
                Address = "Test Address, Test City, Test State - 123456"
            });
        }

        // Mask Aadhaar number for logging (show only last 4 digits)
        private string MaskAadhaar(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar) || aadhaar.Length < 4)
                return "****";
            
            return "********" + aadhaar.Substring(aadhaar.Length - 4);
        }
    }

    // Response models
    public class AadhaarOTPResponse
    {
        public bool Success { get; set; }
        public string? ClientId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MockOTP { get; set; } // Only for development
    }

    public class AadhaarVerifyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
    }
}
