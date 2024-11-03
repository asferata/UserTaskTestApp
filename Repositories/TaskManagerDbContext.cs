using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class TaskManagerDbContext: DbContext
{
    public DbSet<UserTask> UserTasks { get; set; }
    
    protected TaskManagerDbContext()
    {
    }

    public TaskManagerDbContext(DbContextOptions options) : base(options)
    {
    }
}