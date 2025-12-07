using QuizGame.Application.DTO;

namespace QuizGame.Application.Interfaces;

public interface IQuizService
{
    Task<int> AddQuiz(QuizDTO quiz, string UserId);
    Task<bool> UpdateQuiz(QuizDTO quiz);
    Task<bool> DeleteQuiz(int quizId);
    Task<List<ShortQuizInfoDTO>> GetAllQuizes();
    Task<QuizDTO> GetQuiz(int quizId);
}
