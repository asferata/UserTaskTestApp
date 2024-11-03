namespace Utils;

public abstract class IntakerException: Exception
{
    public string Code { get; }

    public IntakerException(string code)
    {
        Code = code;
    }

    public IntakerException(string code, string? message)
        : base(message)
    {
        Code = code;
    }

    public IntakerException(string code, string? message, Exception? innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}