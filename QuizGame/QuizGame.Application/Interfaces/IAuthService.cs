using QuizGame.Application.DTO;

namespace QuizGame.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync (RegisterDTO registerDTO);
    Task<string> LoginAsync(LoginDTO loginDTO);
}