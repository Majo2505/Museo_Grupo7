using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Museo.Data;
using Museo.Repositories;
using Museo.Services;
using Npgsql;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

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


var connectionString = builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrEmpty(connectionString))
{
    var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(dbUrl))
    {
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
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            connectionString = builderDb.ToString();
            Console.WriteLine("--> [DB] Usando DATABASE_URL parseada");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> [DB] Error parseando DATABASE_URL: {ex.Message}");
        }
    }
}

if (string.IsNullOrEmpty(connectionString))
{
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
    if (!string.IsNullOrEmpty(dbHost))
    {
        var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var dbPass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass}";
    }
}

if (!string.IsNullOrEmpty(connectionString))
{
    var debugConn = System.Text.RegularExpressions.Regex.Replace(connectionString, "Password=.*?;", "Password=***;");
    Console.WriteLine($"--> [DB] ConnectionString Final: {debugConn}");
}
else
{
    Console.WriteLine("--> [DB] ¡ERROR! ConnectionString está vacía.");
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

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