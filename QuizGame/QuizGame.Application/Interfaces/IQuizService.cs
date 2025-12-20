using QuizGame.Application.DTO;

namespace QuizGame.Application.Interfaces;

public interface IQuizService
{
    Task<int> AddQuiz(QuizDTO quiz, string UserId);
    Task<bool> UpdateQuiz(QuizDTO quiz, string UserId);
    Task<bool> DeleteQuiz(int quizId, string UserId);
    Task<List<ShortQuizInfoDTO>> GetAllQuizes();
    Task<QuizDTO> GetQuiz(int quizId);
}
