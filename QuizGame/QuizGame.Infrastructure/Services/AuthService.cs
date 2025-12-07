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
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly double _jwtExpireMinutes;
    public AuthService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,string jwtKey, string jwtIssuer, string jwtAudience, string jwtExpiresMinutes)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _jwtKey = jwtKey;
        _jwtIssuer = jwtIssuer;
        _jwtAudience = jwtAudience;
        _jwtExpireMinutes = double.Parse(jwtExpiresMinutes);
    }
    
    public async Task<string> RegisterAsync(RegisterDTO registerDTO)
    {
        var user = new ApplicationUser
        {
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            UserName = registerDTO.Username,
            Email = registerDTO.Email,
        };
        var res = _userManager.CreateAsync(user, registerDTO.Password);
        if (!res.Result.Succeeded)
        {
            var errors = string.Join("; ", res.Result);
            return $"Error: {errors}";
        }
        await _userManager.AddToRoleAsync(user, "User");
        return "User registered successfully";
    }

    public async Task<string> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            throw new Exception("Invalid credentials");

        return await GenerateJwtToken(user);
    }
    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", user.Id)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            _jwtIssuer,
            _jwtAudience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtExpireMinutes),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}