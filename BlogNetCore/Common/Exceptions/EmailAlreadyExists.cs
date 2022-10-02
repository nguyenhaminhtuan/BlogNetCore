namespace BlogNetCore.Common.Exceptions;

public class EmailAlreadyExists : Exception
{
    public EmailAlreadyExists() : base("Email already exists.")
    {
    }

    public EmailAlreadyExists(string message) : base(message)
    {
    }
}