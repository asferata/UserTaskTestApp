using Enums;

namespace Domain;

// Renamed to UserTask in order not to clash with the standard Task class
public class UserTask : Entity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public UserTaskStatus Status { get; set; }
    public string? AssignedTo { get; set; }
}