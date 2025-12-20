using Microsoft.EntityFrameworkCore;
using QuizGame.Application.DTO;
using QuizGame.Application.Interfaces;
using QuizGame.Domain.Entity;

namespace QuizGame.Infrastructure.Services;

public class QuizService:IQuizService
{
    private readonly ApplicationDbContext _context;

    public QuizService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> AddQuiz(QuizDTO quizDto, string userId)
    {
        var quiz = new Quiz
        {
            Name = quizDto.Name,
            Description = quizDto.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            Questions = quizDto.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                Answers = q.Answers.Select(a => new Answer
                {
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                }).ToList()
            }).ToList()
        };

        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();

        return quiz.QuizId; 
    }


   public async Task<bool> UpdateQuiz(QuizDTO quizDto, string userId)
{
    var quiz = await _context.Quizzes
        .Include(q => q.Questions)
            .ThenInclude(qs => qs.Answers)
        .FirstOrDefaultAsync(q => q.QuizId == quizDto.QuizId && q.CreatedBy == userId);

    if (quiz == null)
        return false; 
    
    quiz.Name = quizDto.Name;
    quiz.Description = quizDto.Description;
    quiz.UpdatedAt = DateTime.UtcNow;


    var questionIds = quizDto.Questions.Select(q => q.QuestionId).ToList();
    var questionsToRemove = quiz.Questions
        .Where(q => !questionIds.Contains(q.QuestionId))
        .ToList();

    _context.Questions.RemoveRange(questionsToRemove);


    foreach (var qDto in quizDto.Questions)
    {
        var question = quiz.Questions.FirstOrDefault(q => q.QuestionId == qDto.QuestionId);
        if (question == null)
        {

            question = new Question
            {
                QuestionText = qDto.QuestionText,
                Answers = qDto.Answers.Select(a => new Answer
                {
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };
            quiz.Questions.Add(question);
        }
        else
        {

            question.QuestionText = qDto.QuestionText;

            var answerIds = qDto.Answers.Select(a => a.AnswerId).ToList();
            var answersToRemove = question.Answers
                .Where(a => !answerIds.Contains(a.AnswerId))
                .ToList();
            _context.Answers.RemoveRange(answersToRemove);

            foreach (var aDto in qDto.Answers)
            {
                var answer = question.Answers.FirstOrDefault(a => a.AnswerId == aDto.AnswerId);
                if (answer == null)
                {
                    question.Answers.Add(new Answer
                    {
                        AnswerText = aDto.AnswerText,
                        IsCorrect = aDto.IsCorrect
                    });
                }
                else
                {
                    answer.AnswerText = aDto.AnswerText;
                    answer.IsCorrect = aDto.IsCorrect;
                }
            }
        }
    }

    await _context.SaveChangesAsync();
    return true;
}

    public async Task<bool> DeleteQuiz(int quizId, string UserId)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(qs => qs.Answers)
            .FirstOrDefaultAsync(q => q.QuizId == quizId && q.CreatedBy == UserId);

        if (quiz == null)
            return false;

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ShortQuizInfoDTO>> GetAllQuizes()
    {
        var quizes = await _context.Quizzes
            .Include(q => q.User)
            .ToListAsync();

        var res = quizes.Select(q => new ShortQuizInfoDTO
        {
            QuizId = q.QuizId,
            QuizName = q.Name,
            QuizDescription = q.Description,
            CreatedByName = q.User.UserName,
            CreatedAt = q.CreatedAt,
        }).ToList();

        return res;
    }


    public async Task<QuizDTO> GetQuiz(int quizId)
    {
        var data = await _context.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(qs => qs.Answers)
            .FirstOrDefaultAsync(q => q.QuizId == quizId);
        if (data == null)
            return null;
        var res = new QuizDTO
        {
            QuizId = data.QuizId,
            Name = data.Name,
            Description = data.Description,
            CreatedAt = data.CreatedAt,
            UpdatedAt = data.UpdatedAt,
            CreatedByName = data.User.FirstName + " " + data.User.LastName,
            Questions = data.Questions.Select(q => new QuestionDTO
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                Answers = q.Answers.Select(a => new AnswerDTO
                {
                    AnswerId = a.AnswerId,
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                }).ToList()
            }).ToList()
        };
        return res; 
    }
}