namespace Utils;

public class IntakerNotFoundException : IntakerException
{
    public IntakerNotFoundException(string code)
        : base(code)
    {
    }

    public IntakerNotFoundException(string code, string? message)
        : base(code, message)
    {
    }

    public IntakerNotFoundException(string code, string? message, Exception? innerException)
        : base(code, message, innerException)
    {
    }
}