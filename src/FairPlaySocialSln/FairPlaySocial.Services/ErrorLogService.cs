using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.Services
{
    public class ErrorLogService
    {
        private readonly FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext;

        public ErrorLogService(FairPlaySocialDatabaseContext fairPlaySocialDatabaseContext)
        {
            this.fairPlaySocialDatabaseContext = fairPlaySocialDatabaseContext;
        }

        public async Task <ErrorLog> CreateErrorLogAsync(Exception exception, CancellationToken cancellationToken)
        {
            ErrorLog errorLog = new()
            {
                FullException = exception.ToString(),
                Message = exception.ToString(),
                StackTrace = exception.StackTrace
            };
            await this.fairPlaySocialDatabaseContext.AddAsync(errorLog, cancellationToken);
            await this.fairPlaySocialDatabaseContext.SaveChangesAsync(cancellationToken);
            return errorLog;
        }
    }
}
