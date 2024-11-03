using System.Threading.Tasks;
using Communication.Models;

namespace Communication;

public interface IUserTaskSenderService
{
    Task CreateUserTaskAsync(CreateUserTaskModel model);
    Task UpdateUserTaskAsync(UpdateUserTaskModel model);
}