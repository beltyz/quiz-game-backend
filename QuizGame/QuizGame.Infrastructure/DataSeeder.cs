using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.Domain.Entity;

namespace QuizGame.Infrastructure;


public static class DataSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, string adminEmail, string adminPassword)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newAdmin, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }

}
