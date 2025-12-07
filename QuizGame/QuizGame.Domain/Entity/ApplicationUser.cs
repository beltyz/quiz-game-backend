using Microsoft.AspNetCore.Identity;

namespace QuizGame.Domain.Entity;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}