using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QuizGame.Application.DTO;
using QuizGame.Application.Interfaces;
using QuizGame.Infrastructure;

namespace QuizGame.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private string UserId =>
        User.FindFirstValue("id")
        ?? throw new UnauthorizedAccessException();
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        var res = await _userService.GetUserInfo(UserId);
        return Ok(res);
    }
    [HttpGet("get-all-users-ingo")]
    public async Task<IActionResult> GetAllUsersInfo()
    {
        var res = await _userService.GetAllUsers();
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserDTO userInfo)
    {
        var updated = await _userService.UpdateUserInfo(userInfo, UserId);
        if (!updated) return BadRequest();
        return NoContent();
    }

    [HttpGet("quizzes/{username}")]
    public async Task<IActionResult> GetAllUserQuizzes([FromRoute] string username)
    {
        var res = await _userService.GetAllUsersQuiz(username);
        if (res == null || !res.Any()) return BadRequest();
        return Ok(res);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        var deleted = await _userService.DeleteUser(UserId);
        if (!deleted) return BadRequest();
        return NoContent();
    }
}

    
