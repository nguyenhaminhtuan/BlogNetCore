namespace Api.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Authentication required")
    {
    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}