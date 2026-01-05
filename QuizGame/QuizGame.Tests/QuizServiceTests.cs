using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuizGame.Infrastructure;
using QuizGame.Domain.Entity;
using QuizGame.Infrastructure.Services; 

public class QuizServiceTests
{
    private ApplicationDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetQuiz_ShouldReturnQuizDTO_WhenQuizExists()
    {
        // Arrange
        var context = GetInMemoryDb();

        var quiz = new Quiz
        {
            QuizId = 1,
            Name = "Test Quiz",
            Description = "Desc",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            User = new ApplicationUser { FirstName = "John", LastName = "Doe" },
            Questions = new List<Question>
            {
                new Question
                {
                    QuestionId = 1,
                    QuestionText = "Q1?",
                    Answers = new List<Answer>
                    {
                        new Answer { AnswerId = 1, AnswerText = "A1", IsCorrect = true },
                        new Answer { AnswerId = 2, AnswerText = "A2", IsCorrect = false }
                    }
                }
            }
        };

        context.Quizzes.Add(quiz);
        context.SaveChanges();

        var service = new QuizService(context); // твій сервіс з Application

        // Act
        var result = await service.GetQuiz(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Quiz", result.Name);
        Assert.Single(result.Questions);
        Assert.Equal("John Doe", result.CreatedByName);
    }

    [Fact]
    public async Task GetQuiz_ShouldReturnNull_WhenQuizDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDb();
        var service = new QuizService(context);

        // Act
        var result = await service.GetQuiz(999);

        // Assert
        Assert.Null(result);
    }
}
