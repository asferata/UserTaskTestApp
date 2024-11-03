using Enums;

namespace Communication.Models;

public class UpdateUserTaskModel
{
    public int Id { get; set; }
    public UserTaskStatus Status { get; set; }
}