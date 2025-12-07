namespace QuizGame.Application.DTO;

public class QuestionDTO
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public List<AnswerDTO> Answers { get; set; }
}
