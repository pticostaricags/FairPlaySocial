using FairPlaySocial.AutomatedTests.Services.Providers;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedTests.Services
{
    public class ServicesTestsBase
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;
        protected FairPlaySocialDatabaseContext BuildFairPlaySocialDatabaseContext()
        {
            DbContextOptionsBuilder<FairPlaySocialDatabaseContext>
                optionsBuilder = new();
            optionsBuilder.UseInMemoryDatabase(Constants.Assemblies.MainAppAssemblyName);
            return new FairPlaySocialDatabaseContext(optionsBuilder.Options, new TestsCurrentUserProvider());
        }
    }
}
