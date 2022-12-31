using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.Models.ClientSideErrorLog;

namespace FairPlaySocial.Services
{
    public class ClientSideErrorLogService
    {
        private readonly FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext;

        public ClientSideErrorLogService(FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext)
        {
            this.fairPlaySocialDatabaseContext = fairPlaySocialDatabaseContext;
        }

        public async Task AddClientSideErrorAsync(CreateClientSideErrorLogModel createClientSideErrorLogModel,
            CancellationToken cancellationToken)
        {
            await this.fairPlaySocialDatabaseContext.ClientSideErrorLog.AddAsync(
                new DataAccess.Models.ClientSideErrorLog()
                {
                    FullException = createClientSideErrorLogModel.FullException,
                    Message = createClientSideErrorLogModel.Message,
                    StackTrace = createClientSideErrorLogModel.StackTrace
                }, cancellationToken);
            await this.fairPlaySocialDatabaseContext.SaveChangesAsync(cancellationToken);
        }

    }
}
