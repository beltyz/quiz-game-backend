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
string connectionString;
string jwtIssuer;
string jwtAudience;
string jwtKey;

string AdmPass;
string AdmLogin;

#if DEBUG
DotEnv.Load();
Console.WriteLine("EnterDev");

var port = EnvReader.GetIntValue("POSTGRES_PORT");
var user = EnvReader.GetStringValue("POSTGRES_USER");
var password = EnvReader.GetStringValue("POSTGRES_PASSWORD");
var db = EnvReader.GetStringValue("POSTGRES_DB");

AdmLogin = EnvReader.GetStringValue("ADM_EMAIL");
AdmPass = EnvReader.GetStringValue("ADM_PASSWORD");
connectionString = $"Host=localhost;Port={port};Database={db};Username={user};Password={password}";

// JWT LOCAL
jwtIssuer = EnvReader.GetStringValue("JWT_ISSUER");
jwtAudience = EnvReader.GetStringValue("JWT_AUDIENCE");
jwtKey = EnvReader.GetStringValue("JWT_KEY");

#else
Console.WriteLine("EnterDocker");

// Docker already injects these env vars
connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// JWT DOCKER
jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
AdmLogin = Environment.GetEnvironmentVariable("ADM_EMAIL");
AdmPass = Environment.GetEnvironmentVariable("ADM_PASSWORD");


#endif


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
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
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
var services = scope.ServiceProvider;

var dab = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
try
{
    dab.Database.Migrate();
    Console.WriteLine("✅ Migrations applied successfully");
    
    await DataSeeder.SeedRolesAndAdminAsync(services, AdmLogin, AdmPass);
    Console.WriteLine("✅ Roles and admin seeded successfully");
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