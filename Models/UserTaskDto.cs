using Domain;
using Enums;

namespace Models;

public class UserTaskDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public UserTaskStatus Status { get; set; }
    public string? AssignedTo { get; set; }
}