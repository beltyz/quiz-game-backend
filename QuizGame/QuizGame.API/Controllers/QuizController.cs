using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizGame.Application.DTO;
using QuizGame.Application.Interfaces;
using QuizGame.Infrastructure;

namespace QuizGame.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizController:ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly ApplicationDbContext _context;

    public QuizController(IQuizService quizService,  ApplicationDbContext context)
    {
        _quizService = quizService;
        _context = context;
    }

    [HttpPost("add-quiz")]
    public async Task<IActionResult> AddQuiz([FromBody] QuizDTO quizDTO)
    {
        var userId = User.FindFirstValue("id");
        if (userId == null)
            return Unauthorized();
        
        var res = await _quizService.AddQuiz(quizDTO, userId);
        return Ok(res);
    }
    [HttpPut("update-quiz")]
    public async Task<IActionResult> UpdateQuiz([FromBody] QuizDTO quizDTO)
    {
        var userId = User.FindFirstValue("id");
        if (userId == null)
            return Unauthorized();
        
        var res = await _quizService.UpdateQuiz(quizDTO, userId);
        return Ok(res);
    }
    [HttpDelete("delete-quiz")]
    public async Task<IActionResult> DeleteQuiz([FromBody] int QuizId)
    {
        var userId = User.FindFirstValue("id");
        if (userId == null)
            return Unauthorized();
        
        var res = await _quizService.DeleteQuiz(QuizId, userId);
        return Ok(res);
    }

    [HttpGet("get-all-quizes")]
    public async Task<List<ShortQuizInfoDTO>> GetAllQuizes()
    {
        var quizes = await _quizService.GetAllQuizes();
        return quizes;
    }

    [HttpGet("get-quiz-by-id")]
    public async Task<ActionResult<QuizDTO>> GetQuizById(int id)
    {
        var res = await _quizService.GetQuiz(id);
        if (res == null)
            return NotFound();

        return Ok(res);
    }

    
    
}