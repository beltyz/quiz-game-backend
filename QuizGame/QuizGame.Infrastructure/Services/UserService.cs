using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizGame.Application.DTO;
using QuizGame.Application.Interfaces;

namespace QuizGame.Infrastructure.Services;

public class UserService:IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task<UserDTO?> GetUserInfo(string UserId)
    {
        return await _context.Users
            .Where(u => u.Id == UserId)
            .Select(u => new UserDTO
            {
                UserId = u.Id,
                Email = u.Email,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateUserInfo(UserDTO user, string UserId)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(e => e.Id == UserId);

       
        if (existingUser == null)
            return false;

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.UserName = user.Username;

        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> DeleteUser(string UserId)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == UserId);

        if (existingUser == null)
            return false;
        
        _context.Users.Remove(existingUser);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ShortQuizInfoDTO>> GetAllUsersQuiz(string username)
    {
        return await _context.Quizzes
            .Where(i => i.User.UserName == username)
            .Include(i => i.User)
            .Select(c => new ShortQuizInfoDTO
            {
                QuizId = c.QuizId,
                QuizName = c.Name,
                QuizDescription = c.Description,
                CreatedAt = c.CreatedAt,
                CreatedByName = c.User.UserName
            }).ToListAsync();
    }

    public async Task<List<UserDTO>> GetAllUsers()
    {
        return await _context.Users
            .Select(u => new UserDTO
            {
                UserId = u.Id,
                Email = u.Email,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
            })
            .ToListAsync();    
    }
}