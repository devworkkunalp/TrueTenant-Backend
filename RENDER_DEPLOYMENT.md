# TrueTenant Backend - Render Deployment Guide

## 🐳 Docker Deployment on Render

### Prerequisites
- Render account (free tier available)
- GitHub repository: https://github.com/devworkkunalp/TrueTenant-Backend

---

## 📋 Step-by-Step Deployment

### Step 1: Create PostgreSQL Database on Render

1. **Go to [Render Dashboard](https://dashboard.render.com)**

2. **Create New PostgreSQL Database:**
   - Click "New +" → "PostgreSQL"
   - Name: `truetenant-db`
   - Database: `truetenant`
   - User: `truetenant_user`
   - Region: Choose closest to you
   - Plan: Free (or paid for production)
   - Click "Create Database"

3. **Copy Connection Details:**
   - Internal Database URL (for connecting from Render services)
   - External Database URL (for local testing)
   - Save these for later

---

### Step 2: Update Backend for PostgreSQL

Since Render uses PostgreSQL (not SQL Server), update your code:

#### Install PostgreSQL Package
```bash
cd Server
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

#### Update Program.cs
Replace the SQL Server configuration with PostgreSQL:

```csharp
// Before builder.Build(), replace DbContext configuration:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); // Changed from UseSqlServer
```

---

### Step 3: Deploy Backend Web Service

1. **Create New Web Service:**
   - Click "New +" → "Web Service"
   - Connect your GitHub: `devworkkunalp/TrueTenant-Backend`
   - Click "Connect"

2. **Configure Service:**
   - **Name**: `truetenant-api`
   - **Region**: Same as database
   - **Branch**: `main`
   - **Root Directory**: Leave empty (or `.` if needed)
   - **Environment**: `Docker`
   - **Dockerfile Path**: `Dockerfile` (Render will auto-detect)
   - **Plan**: Free (or paid for production)

3. **Add Environment Variables:**
   Click "Advanced" → "Add Environment Variable"
   
   Add these variables:
   ```
   ConnectionStrings__DefaultConnection
   Value: <Paste Internal Database URL from Step 1>
   
   Jwt__Key
   Value: YourSuperSecretKeyHere_MakeItLongEnoughForSecurityReasons_ChangeThisToSomethingRandom64Chars
   
   Jwt__Issuer
   Value: TrueTenantAPI
   
   Jwt__Audience
   Value: TrueTenantClient
   
   ASPNETCORE_ENVIRONMENT
   Value: Production
   
   AadhaarAPI__Enabled
   Value: false
   
   AadhaarAPI__BaseUrl
   Value: https://sandbox.surepass.io/api/v1
   
   AadhaarAPI__ApiKey
   Value: YOUR_API_KEY_WHEN_READY
   ```

4. **Deploy:**
   - Click "Create Web Service"
   - Render will build your Docker image and deploy
   - Wait for deployment to complete (5-10 minutes)

5. **Get Your Backend URL:**
   - After deployment, you'll see your URL
   - Example: `https://truetenant-api.onrender.com`
   - Copy this for frontend configuration

---

### Step 4: Run Database Migrations

After first deployment, you need to run migrations:

1. **Option A: Using Render Shell**
   - Go to your web service dashboard
   - Click "Shell" tab
   - Run:
     ```bash
     dotnet ef database update
     ```

2. **Option B: Automatic Migration on Startup**
   Update `Program.cs` to run migrations automatically:
   ```csharp
   // After var app = builder.Build();
   using (var scope = app.Services.CreateScope())
   {
       var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
       db.Database.Migrate(); // Auto-migrate on startup
   }
   ```

---

### Step 5: Update CORS for Frontend

In `Program.cs`, update CORS to allow your frontend:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5174",  // Local development
            "https://your-frontend.vercel.app"  // Production frontend
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
```

Commit and push to trigger redeployment.

---

## 🧪 Testing Your Deployed API

### Test Health
```bash
curl https://truetenant-api.onrender.com/api/auth/login
```

### Test with Postman
- URL: `https://truetenant-api.onrender.com/api`
- Try register/login endpoints

---

## 📝 Local Docker Testing

Before deploying, test locally:

```bash
# Build Docker image
docker build -t truetenant-backend .

# Run container
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Your_Local_Connection" \
  -e Jwt__Key="YourSecretKey" \
  -e Jwt__Issuer="TrueTenantServer" \
  -e Jwt__Audience="TrueTenantClient" \
  truetenant-backend

# Or use docker-compose
docker-compose up
```

Access at: `http://localhost:8080`

---

## 🔄 Continuous Deployment

Render automatically deploys when you push to `main`:

```bash
cd Server
git add .
git commit -m "Update backend"
git push origin main
```

Render will:
1. Detect the push
2. Build new Docker image
3. Deploy automatically
4. Zero-downtime deployment

---

## 🐛 Troubleshooting

### Build Fails
- Check Dockerfile syntax
- Ensure all dependencies in .csproj
- Check Render build logs

### Database Connection Fails
- Verify connection string format
- Check PostgreSQL is running
- Ensure database and web service in same region

### Port Issues
- Render expects port 8080 (configured in Dockerfile)
- Don't change EXPOSE port

### Migration Errors
- Run migrations manually via Shell
- Or enable auto-migration in Program.cs

---

## 💰 Render Pricing

### Free Tier
- ✅ 750 hours/month
- ✅ Automatic HTTPS
- ✅ Auto-deploy from Git
- ⚠️ Spins down after 15 min inactivity
- ⚠️ Cold start ~30 seconds

### Paid Tier ($7/month)
- ✅ Always on
- ✅ No cold starts
- ✅ Better performance

---

## 📊 Monitoring

### Render Dashboard
- Real-time logs
- Metrics (CPU, Memory)
- Deployment history
- Shell access

### Logs
```bash
# View in dashboard or via CLI
render logs -f
```

---

## 🔒 Security Checklist

- [x] Strong JWT secret (64+ characters)
- [x] HTTPS enabled (automatic on Render)
- [x] Environment variables (not hardcoded)
- [x] CORS configured for specific domains
- [x] Database SSL enabled
- [ ] Rate limiting (add if needed)
- [ ] API key rotation policy

---

## 🎯 Next Steps

1. ✅ Backend deployed on Render
2. ⏭️ Deploy frontend to Vercel/Netlify
3. ⏭️ Update frontend `VITE_API_URL` with Render URL
4. ⏭️ Test complete flow
5. ⏭️ Set up custom domain (optional)

---

## 📞 Support

- Render Docs: https://render.com/docs
- GitHub Issues: https://github.com/devworkkunalp/TrueTenant-Backend/issues

---

## ✅ Deployment Checklist

- [ ] PostgreSQL database created
- [ ] Environment variables configured
- [ ] Dockerfile in repository
- [ ] Code pushed to GitHub
- [ ] Web service created on Render
- [ ] Build successful
- [ ] Migrations applied
- [ ] API responding
- [ ] CORS configured
- [ ] Frontend URL updated

You're ready to deploy! 🚀
