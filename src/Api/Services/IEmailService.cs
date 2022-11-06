namespace Api.Services;

public interface IEmailService
{
    public Task<bool> Send(string from, string to, string subject, string content);
}