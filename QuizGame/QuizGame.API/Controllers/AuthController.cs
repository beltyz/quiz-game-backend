using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QuizGame.Application.DTO;
using QuizGame.Application.Interfaces;

namespace QuizGame.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController:ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        var res = await _authService.RegisterAsync(registerDTO);
        
        return Ok(res);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var res = await _authService.LoginAsync(login);
        
        return Ok(res);
    }

}