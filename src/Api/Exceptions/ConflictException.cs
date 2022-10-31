namespace Api.Exceptions;

public class ConflictException : Exception
{
    public ConflictException() : base("Resource conflict")
    {
    }

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}