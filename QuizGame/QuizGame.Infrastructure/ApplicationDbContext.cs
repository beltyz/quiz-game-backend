using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizGame.Domain.Entity;

namespace QuizGame.Infrastructure;

public class ApplicationDbContext:IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public DbSet<Quiz>  Quizzes { get; set; }
    public DbSet<Question>  Questions { get; set; }
    public DbSet<Answer>  Answers { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

}