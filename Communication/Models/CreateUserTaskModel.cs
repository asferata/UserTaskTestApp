namespace Communication.Models;

public class CreateUserTaskModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? AssignedTo { get; set; }
}