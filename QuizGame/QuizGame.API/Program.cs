using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuizGame.Domain.Entity;
using QuizGame.Infrastructure;
using System.Text;
using QuizGame.Application.Interfaces;
using QuizGame.Infrastructure.Services;
using dotenv.net;
using dotenv.net.Utilities;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Configuration.AddEnvironmentVariables();
#if DEBUG
// Локально читаємо .env через DotEnv
DotEnv.Load();
Console.WriteLine("EnterDev");

var port = EnvReader.GetIntValue("POSTGRES_PORT");
var user = EnvReader.GetStringValue("POSTGRES_USER");
var password = EnvReader.GetStringValue("POSTGRES_PASSWORD");
var db = EnvReader.GetStringValue("POSTGRES_DB");

Console.WriteLine($"POSTGRES_PORT: {port}");
Console.WriteLine($"POSTGRES_USER: {user}");
Console.WriteLine($"POSTGRES_PASSWORD: {password}");
Console.WriteLine($"POSTGRES_DB: {db}");

// Формуємо connection string локально
var connectionString = $"Host=localhost;Port={port};Database={db};Username={user};Password={password}";
#else
Console.WriteLine("EnterDocker");
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
#endif

Console.WriteLine($"connectionString: {connectionString}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireDigit = false;         
        options.Password.RequiredLength = 4;           
        options.Password.RequireLowercase = false;       
        options.Password.RequireUppercase = false;      
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 0;    
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// Authentication + JWT
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
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = EnvReader.GetStringValue("JWT_ISSUER"),
            ValidAudience = EnvReader.GetStringValue("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(EnvReader.GetStringValue("JWT_KEY")))
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IQuizService, QuizService>();

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using var scope = app.Services.CreateScope();
var dab = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
try
{
    dab.Database.Migrate();
    Console.WriteLine("✅ Migrations applied successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Migration failed: {ex.Message}");
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();