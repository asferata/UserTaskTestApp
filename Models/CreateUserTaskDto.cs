using System.ComponentModel.DataAnnotations;

namespace Models;

public class CreateUserTaskDto
{
    [Required(ErrorMessage = "Name is required", AllowEmptyStrings = false)]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Description is required", AllowEmptyStrings = false)]
    public required string Description { get; set; }
    public string? AssignedTo { get; set; }
}