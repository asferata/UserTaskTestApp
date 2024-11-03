using Domain;
using Enums;
using Repositories;
using Utils;

namespace Services.BusinessLogic.Implementation;

public class UserTaskService(IUserTaskRepository repository) : IUserTaskService
{
    public async Task<List<UserTask>> PageAsync(int offset = 0, int count = int.MaxValue)
    {
        return await repository.PageAsync(offset, count);
    }

    public async Task<UserTask?> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<UserTask> CreateAsync(UserTask entity)
    {
        entity.Status = UserTaskStatus.NotStarted;
        return await repository.CreateAsync(entity);
    }

    public async Task<UserTask> UpdateStatusAsync(int id, UserTaskStatus status)
    {
        var existingEntity = await repository.GetByIdAsync(id);
        if (existingEntity == null)
        {
            throw new IntakerNotFoundException(ErrorCodes.UserTaskNotFound.Code,
                ErrorCodes.UserTaskNotFound.Description);
        }

        // Not validating a status here
        existingEntity.Status = status;
        return await repository.UpdateAsync(existingEntity);
    }
}