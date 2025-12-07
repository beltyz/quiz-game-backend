using QuizGame.Domain.Entity;

namespace QuizGame.Application.DTO;

public class QuizDTO
{
    public int QuizId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedByName { get; set; }
    public List<QuestionDTO> Questions { get; set; }
}
