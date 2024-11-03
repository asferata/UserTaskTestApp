using Enums;

namespace Models;

public class UpdateUserTaskDto
{
    public required UserTaskStatus Status { get; set; }
}