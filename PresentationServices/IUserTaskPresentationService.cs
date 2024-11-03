using Models;

namespace PresentationServices;

public interface IUserTaskPresentationService
{
    Task<List<UserTaskDto>> PageAsync(int offset = 0, int count = int.MaxValue);
    Task CreateAsync(CreateUserTaskDto dto);
    Task UpdateAsync(int id, UpdateUserTaskDto dto);
}