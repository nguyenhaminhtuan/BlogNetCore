namespace Api.Services;

public class FakeEmailService : IEmailService
{
    private readonly ILogger<FakeEmailService> _logger;
    
    public FakeEmailService(ILogger<FakeEmailService> logger)
    {
        _logger = logger;
    }

    public Task<bool> Send(string from, string to, string subject, string content)
    {
        _logger.LogInformation("From: {EmailFrom}; To {EmailTo}", from, to);
        _logger.LogInformation("Subject: {EmailSubject}", subject);
        _logger.LogInformation("Content: {EmailContent}", content);
        return Task.FromResult(true);
    }
}