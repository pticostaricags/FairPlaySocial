using FairPlaySocial.Common.Interfaces.Services;

namespace FairPlaySocial.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string toEmailAddress, string subject, string body, bool isBodyHtml, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
