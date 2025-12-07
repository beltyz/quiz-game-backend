using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotenv.net.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QuizGame.Application.DTO;
using QuizGame.Application.Interfaces;
using QuizGame.Domain.Entity;

namespace QuizGame.Infrastructure.Services;

public class AuthService: IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<string> RegisterAsync(RegisterDTO registerDTO)
    {
        var user = new ApplicationUser
        {
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            UserName = registerDTO.Username,
        };
        var res = _userManager.CreateAsync(user, registerDTO.Password);
        if (!res.Result.Succeeded)
        {
            var errors = string.Join("; ", res.Result);
            return $"Error: {errors}";
        }

        return "User registered successfully";
    }

    public async Task<string> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByNameAsync(loginDTO.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            throw new Exception("Invalid credentials");

        return await GenerateJwtToken(user);
    }
    private Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var key = EnvReader.GetStringValue("JWT_KEY");
        var issuer = EnvReader.GetStringValue("JWT_ISSUER");
        var audience = EnvReader.GetStringValue("JWT_AUDIENCE");
        var expireMinutes = EnvReader.GetIntValue("JWT_EXPIRE_MINUTES");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", user.Id)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256)
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}