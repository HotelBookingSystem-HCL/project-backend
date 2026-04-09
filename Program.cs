// Program.cs
// This is the entry point of the application.
// We register all services, configure middleware, and set up the HTTP pipeline here.

using HotelBooking.Configurations;
using HotelBooking.Data;
using HotelBooking.Helpers;
using HotelBooking.Interfaces;
using HotelBooking.Middleware;
using HotelBooking.Repositories;
using HotelBooking.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────
// 1. ADD SERVICES TO THE DEPENDENCY INJECTION CONTAINER
// ─────────────────────────────────────────────────────────────

// Add controllers (enables our API endpoints)
builder.Services.AddControllers();

// Add Swagger/OpenAPI (for testing the API in the browser)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Booking API",
        Version = "v1",
        Description = "Full-Stack .NET Hotel Booking System - Case 3"
    });

    // Allow sending JWT token from Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here. Example: Bearer eyJhbGciOi..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ─── Database ───────────────────────────────────────────────
// Register Entity Framework with MySQL (Pomelo provider)
// Connection string comes from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// ─── JWT Configuration ────────────────────────────────────────
// Read JWT settings from appsettings.json and register as a singleton
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new Exception("JwtSettings not found in appsettings.json");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<JwtHelper>();

// Tell ASP.NET to use JWT Bearer tokens for authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,          // Token must not be expired
        ValidateIssuerSigningKey = true,  // Token must be signed with our secret
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// ─── Authorization Policies ──────────────────────────────────
builder.Services.AddAuthorization();

// ─── Repositories (Database Access Layer) ────────────────────
// "AddScoped" = one instance per HTTP request (perfect for DB access)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// ─── Services (Business Logic Layer) ─────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// ─── CORS (Allow frontend to call this API) ──────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ─────────────────────────────────────────────────────────────
// 2. BUILD THE APP AND CONFIGURE THE HTTP PIPELINE
// ─────────────────────────────────────────────────────────────

var app = builder.Build();

// ─── Auto-apply migrations on startup ────────────────────────
// This creates/updates the database tables automatically


// ─── Middleware Pipeline (ORDER MATTERS!) ─────────────────────

// 1. Our custom error handler - catches all exceptions globally
app.UseMiddleware<ExceptionMiddleware>();

// 2. Swagger UI - only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Booking API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger at root URL "/"
    });
}

// 3. HTTPS redirect
app.UseHttpsRedirection();

// 4. CORS - must be before Auth
app.UseCors("AllowAll");

// 5. Authentication (Who are you?) - must come before Authorization
app.UseAuthentication();

// 6. Authorization (Are you allowed?) - must come after Authentication
app.UseAuthorization();

// 7. Map controller routes
app.MapControllers();

app.Run();