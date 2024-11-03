using Communication;
using Communication.Models;
using Domain;
using Enums;
using Models;
using Services.BusinessLogic;
using Utils;

namespace PresentationServices.Implementation;

public class UserTaskPresentationService(IUserTaskSenderService userTaskSenderService,
    IUserTaskService userTaskService) : IUserTaskPresentationService
{
    public async Task<List<UserTaskDto>> PageAsync(int offset = 0, int count = int.MaxValue)
    {
        if (offset < 0)
        {
            throw new IntakerValidationException(ErrorCodes.InvalidParameters.Code, "Offset must be greater or equal 0");
        }
        
        if (count < 0)
        {
            throw new IntakerValidationException(ErrorCodes.InvalidParameters.Code, "Count must be greater or equal 0");
        }

        var entities = await userTaskService.PageAsync(offset, count);
        return entities.Select(ToDto).ToList();
    }

    public async Task CreateAsync(CreateUserTaskDto dto)
    {
        var model = ToModel(dto);

        await userTaskSenderService.CreateUserTaskAsync(model);
    }

    public async Task UpdateAsync(int id, UpdateUserTaskDto dto)
    {
        var existingEntity = await userTaskService.GetByIdAsync(id);
        if (existingEntity == null)
        {
            throw new IntakerNotFoundException(ErrorCodes.UserTaskNotFound.Code,
                ErrorCodes.UserTaskNotFound.Description);
        }
        
        await userTaskSenderService.UpdateUserTaskAsync(ToModel(id, dto.Status));
    }

    private static UserTaskDto ToDto(UserTask entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            AssignedTo = entity.AssignedTo
        };
    }

    private static CreateUserTaskModel ToModel(CreateUserTaskDto dto)
    {
        return new()
        {
            Name = dto.Name,
            Description = dto.Description,
            AssignedTo = dto.AssignedTo
        };
    }

    private static UpdateUserTaskModel ToModel(int id, UserTaskStatus status)
    {
        return new()
        {
            Id = id,
            Status = status
        };
    }
}