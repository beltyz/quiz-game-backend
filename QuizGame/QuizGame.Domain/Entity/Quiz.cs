using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGame.Domain.Entity;

[Table("Quizzes")]
public class Quiz
{
    public int QuizId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [Required]
    public string CreatedBy { get; set; }
    [ForeignKey("CreatedBy")]
    public ApplicationUser User { get; set; }
    public List<Question> Questions { get; set; }
    
}