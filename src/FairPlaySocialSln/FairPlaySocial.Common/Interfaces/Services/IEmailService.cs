namespace FairPlaySocial.Common.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmailAddress, string subject, string body, bool isBodyHtml, CancellationToken cancellationToken);
    }
}
