namespace Models;

public class ErrorDto(int statusCode, string code, string message)
{
    public int StatusCode { get; set; } = statusCode;
    
    public string Code { get; set; } = code;

    public string Message { get; set; } = message;
}