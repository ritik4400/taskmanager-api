using Microsoft.EntityFrameworkCore;

namespace taskmanager_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet<User> and DbSet<TaskItem> will be added in the next step
}