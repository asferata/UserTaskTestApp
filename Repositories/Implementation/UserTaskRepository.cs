using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Implementation;

public class UserTaskRepository(TaskManagerDbContext dbContext)
    : EntityRepository<UserTask>(dbContext), IUserTaskRepository
{

    public override async Task<List<UserTask>> PageAsync(int offset = 0, int count = int.MaxValue)
    {
        return await DbContext.UserTasks
            .OrderBy(it => it.Name)
            .ThenBy(it => it.Id)
            .Skip(offset)
            .Take(count)
            .ToListAsync();
    }
}