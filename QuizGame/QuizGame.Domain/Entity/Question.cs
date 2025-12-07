using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGame.Domain.Entity;

[Table("Questions")]
public class Question
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public int QuizId { get; set; }
    [ForeignKey("QuizId")]
    public Quiz Quiz { get; set; }
    public List<Answer> Answers { get; set; }
    
}