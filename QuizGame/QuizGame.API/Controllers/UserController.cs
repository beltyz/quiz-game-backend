using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuizGame.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController
{
    
}