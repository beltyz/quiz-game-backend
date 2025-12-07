using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGame.Domain.Entity;

[Table("Answers")]
public class Answer
{
    
    public int AnswerId { get; set; }
    [Required]
    public string AnswerText { get; set; }
    [Required]
    public bool IsCorrect { get; set; }
    [Required]
    public int QuestionId { get; set; }
    [ForeignKey("QuestionId")]
    public Question Question { get; set; }
}