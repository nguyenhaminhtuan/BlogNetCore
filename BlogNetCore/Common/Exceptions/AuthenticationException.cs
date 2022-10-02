namespace BlogNetCore.Common.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException() : base("Unauthenticated.") { }

    public AuthenticationException(string message)
        : base(message)
    {
    }
}