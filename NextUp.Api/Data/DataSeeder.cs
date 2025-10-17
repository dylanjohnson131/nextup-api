using Microsoft.EntityFrameworkCore;
using NextUp.Models;
using NextUp.Api.Services;
namespace NextUp.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(NextUpDbContext context, IPasswordService passwordService)
        {
            // Only seed an initial athletic director if none exists
            if (!await context.Users.AnyAsync(u => u.Role == User.ATHLETIC_DIRECTOR_ROLE))
            {
                var athleticDirectorUser = new User
                {
                    FirstName = "Admin",
                    LastName = "Director",
                    Email = "admin@nextup.com",
                    PasswordHash = passwordService.HashPassword("admin123"),
                    Role = User.ATHLETIC_DIRECTOR_ROLE,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Users.Add(athleticDirectorUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
