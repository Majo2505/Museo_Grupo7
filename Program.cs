using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Museo.Data;
using Museo.Repositories;
using Museo.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Npgsql; // Asegúrate de tener instalado Npgsql

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde .env si existe
Env.Load();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// --- Servicios Básicos ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Rate Limiter ---
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });
});

// --- CORS ---
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// --- Autenticación JWT ---
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

var keyBytes = Encoding.UTF8.GetBytes(jwtKey ?? "ClaveSecretaSuperSeguraParaDesarrollo12345!");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

// --- BASE DE DATOS (Lógica Robusta Híbrida) ---

// 1. Intento inicial: Leer de appsettings.json (Local)
var connectionString = builder.Configuration.GetConnectionString("Default");

// 2. Verificar si existe DATABASE_URL (Railway / Producción)
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(dbUrl))
{
    // Si hay URL de Railway, la parseamos para convertirla a formato Npgsql
    try
    {
        var databaseUri = new Uri(dbUrl);
        var userInfo = databaseUri.UserInfo.Split(':');

        var builderDb = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require, // Railway requiere SSL usualmente
            TrustServerCertificate = true
        };
        connectionString = builderDb.ToString();
        Console.WriteLine("--> [DB] Usando configuración de Railway/Nube");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> [DB] Error parseando DATABASE_URL: {ex.Message}");
    }
}
else if (string.IsNullOrEmpty(connectionString))
{
    // 3. Fallback: Construcción manual para Docker si no hay appsettings ni Railway
    var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
    var dbPass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";

    if (!string.IsNullOrEmpty(dbName))
    {
        connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass}";
        Console.WriteLine("--> [DB] Usando configuración de Variables de Entorno (Docker)");
    }
}

// Debug para desarrollo
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine($"[DEBUG] ConnectionString Final: {connectionString}");
}

// Inyección del Contexto
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// --- Repositorios y Servicios ---
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<ICanvasRepository, CanvasRepository>();
builder.Services.AddScoped<IWorkRepository, WorkRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IMuseumRepository, MuseumRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<ICanvasService, CanvasService>();
builder.Services.AddScoped<IWorkService, WorkService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IMuseumService, MuseumService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// --- Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();