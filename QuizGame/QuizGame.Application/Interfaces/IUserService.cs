using QuizGame.Application.DTO;

namespace QuizGame.Application.Interfaces;

public interface IUserService
{
    Task<UserDTO?> GetUserInfo(string UserId);
    Task<bool> UpdateUserInfo(UserDTO user, string UserId);
    Task<bool> DeleteUser(string UserId);
    Task<List<ShortQuizInfoDTO>> GetAllUsersQuiz(string username);
}