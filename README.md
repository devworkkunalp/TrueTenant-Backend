# TrueTenant Backend API

.NET Core Web API backend for the TrueTenant property management platform.

## 🚀 Features

- **RESTful API**: Clean, well-documented API endpoints
- **KYC Verification**: Aadhaar verification service integration
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Authorization**: Owner and Tenant roles
- **Entity Framework Core**: Database management with migrations
- **SQL Server**: Robust database support

## 🛠️ Tech Stack

- .NET Core 9.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger/OpenAPI

## 📋 Prerequisites

- .NET SDK 9.0+
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code

## 🔧 Installation

```bash
# Clone the repository
git clone https://github.com/devworkkunalp/TrueTenant-Backend.git
cd TrueTenant-Backend

# Restore dependencies
dotnet restore

# Update database connection string in appsettings.json
# Then run migrations
dotnet ef database update

# Run the application
dotnet run
```

The API will run on `http://localhost:5170`

## 📁 Project Structure

```
├── Controllers/         # API Controllers
│   ├── AuthController.cs
│   ├── PropertiesController.cs
│   ├── RequestsController.cs
│   ├── PaymentsController.cs
│   └── KYCController.cs
├── Models/             # Data Models
│   ├── User.cs
│   ├── Property.cs
│   ├── MaintenanceRequest.cs
│   ├── Payment.cs
│   └── KYCDocument.cs
├── Data/               # Database Context
│   └── ApplicationDbContext.cs
├── Services/           # Business Logic
│   └── AadhaarVerificationService.cs
├── Migrations/         # EF Core Migrations
└── Program.cs          # Application Entry Point
```

## 🗄️ Database Schema

### Users
- Id, Name, Email, PasswordHash, Role
- KYCStatus, AadhaarVerified, PANVerified
- KYCSubmittedAt, KYCVerifiedAt

### Properties
- Id, Title, Address, RentAmount, Status
- OwnerId, TenantId, ImageUrl

### MaintenanceRequests
- Id, Title, Description, Type, Priority, Status
- PropertyId, TenantId, CreatedAt

### Payments
- Id, Amount, Date, Method, Description
- PropertyId, TenantId

### KYCDocuments
- Id, UserId, DocumentType, DocumentNumber
- VerificationStatus, VerifiedName, DateOfBirth, Gender
- UploadedAt, VerifiedAt

## 🔐 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Properties
- `GET /api/properties` - Get all properties
- `GET /api/properties/owner` - Get owner's properties
- `POST /api/properties` - Create property (Owner only)
- `GET /api/properties/{id}` - Get property details

### Maintenance Requests
- `GET /api/requests` - Get requests
- `POST /api/requests` - Create request (Tenant only)
- `PUT /api/requests/{id}/status` - Update request status (Owner only)

### Payments
- `GET /api/payments` - Get payments
- `POST /api/payments` - Record payment (Tenant only)

### KYC
- `POST /api/kyc/aadhaar/generate-otp` - Generate Aadhaar OTP
- `POST /api/kyc/aadhaar/verify-otp` - Verify Aadhaar OTP
- `GET /api/kyc/status` - Get KYC status
- `GET /api/kyc/documents` - Get KYC documents

## 🔑 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TrueTenantDB;Trusted_Connection=True;Encrypt=False;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHere_MakeItLongEnoughForSecurityReasons",
    "Issuer": "TrueTenantServer",
    "Audience": "TrueTenantClient"
  },
  "AadhaarAPI": {
    "Enabled": false,
    "BaseUrl": "https://sandbox.surepass.io/api/v1",
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

## 🗃️ Database Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 🔗 Frontend Repository

Frontend App: [TrueTenant-Frontend](https://github.com/devworkkunalp/TrueTenant-Frontend)

## 🚀 Deployment

### Railway (Recommended)

1. Push to GitHub
2. Create new project in Railway
3. Add PostgreSQL database
4. Set environment variables
5. Deploy

### Azure

```bash
# Publish
dotnet publish -c Release

# Deploy to Azure App Service
az webapp up --name truetenant-api --resource-group TrueTenantRG
```

## 📝 Environment Variables (Production)

```
ConnectionStrings__DefaultConnection=<Production_DB_Connection>
Jwt__Key=<Strong_Secret_Key>
Jwt__Issuer=TrueTenantAPI
Jwt__Audience=TrueTenantClient
ASPNETCORE_ENVIRONMENT=Production
AadhaarAPI__Enabled=true
AadhaarAPI__ApiKey=<Real_API_Key>
```

## 🧪 Testing

```bash
# Run tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📚 API Documentation

Once running, visit:
- Swagger UI: `http://localhost:5170/swagger`
- API Docs: `http://localhost:5170/api-docs`

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

MIT License

## 👨‍💻 Author

Kunal Patil - [GitHub](https://github.com/devworkkunalp)
