namespace Utils;

public class ErrorCodes
{
    public static ErrorCodes General = new("IN-000-001", "General error");
    public static ErrorCodes InvalidParameters = new("IN-000-002", "Invalid parameters");
    public static ErrorCodes UserTaskNotFound = new("IN-000-003", "UserTask entity not found");
    public static ErrorCodes CreateUserTaskSendError = new("IN-000-004", "UserTask entity was not created due to a queue error");
    public static ErrorCodes UpdateUserTaskSendError = new("IN-000-005", "UserTask entity was not update due to a queue error");
    
    public string Code { get; }
    public string Description { get; }

    private ErrorCodes(string code, string description)
    {
        Code = code;
        Description = description;
    }
}