using Microsoft.EntityFrameworkCore;

namespace NextUp.Api.Data;

public class NextUpDbContext : DbContext
{
    public NextUpDbContext(DbContextOptions<NextUpDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}
