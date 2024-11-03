namespace Utils;

public class IntakerInternalErrorException : IntakerException
{
    public IntakerInternalErrorException(string code)
        : base(code)
    {
    }

    public IntakerInternalErrorException(string code, string? message)
        : base(code, message)
    {
    }

    public IntakerInternalErrorException(string code, string? message, Exception? innerException)
        : base(code, message, innerException)
    {
    }
    
    public IntakerInternalErrorException(ErrorCodes error)
        : base(error.Code, error.Description)
    {
    }

    public IntakerInternalErrorException(ErrorCodes error, Exception? innerException)
        : base(error.Code, error.Description, innerException)
    {
    }
}