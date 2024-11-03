namespace Utils;

public class IntakerValidationException : IntakerException
{
    public IntakerValidationException(string code)
        : base(code)
    {
    }

    public IntakerValidationException(string code, string? message)
        : base(code, message)
    {
    }

    public IntakerValidationException(string code, string? message, Exception? innerException)
        : base(code, message, innerException)
    {
    }
}