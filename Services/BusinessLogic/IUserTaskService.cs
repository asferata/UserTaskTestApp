using Domain;
using Enums;

namespace Services.BusinessLogic;

public interface IUserTaskService
{
    Task<List<UserTask>> PageAsync(int offset = 0, int count = int.MaxValue);
    Task<UserTask?> GetByIdAsync(int id);
    Task<UserTask> CreateAsync(UserTask entity);
    Task<UserTask> UpdateStatusAsync(int id, UserTaskStatus status);
}