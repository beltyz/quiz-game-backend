using QuizGame.Application.DTO;

namespace QuizGame.Application.Interfaces;

public interface IUserService
{
    Task<UserDTO> GetUserInfo(string username);
    Task<bool> UpdateUserInfo(UserDTO user, string username);
    Task<bool> DeleteUser(string username);
    Task<List<ShortQuizInfoDTO>> GetAllUsersQuiz(string username);
}