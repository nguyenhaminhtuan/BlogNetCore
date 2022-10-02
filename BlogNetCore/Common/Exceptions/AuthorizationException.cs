namespace BlogNetCore.Common.Exceptions;

public class AuthorizationException : Exception
{
    public AuthorizationException() : base("Access Denied.") {}

    public AuthorizationException(string message) 
        : base(message)
    {
    }
}