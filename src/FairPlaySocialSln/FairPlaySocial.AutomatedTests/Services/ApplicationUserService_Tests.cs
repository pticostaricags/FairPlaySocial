using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedTests.Services
{
    [TestClass]
    public class ApplicationUserService_Tests: ServicesTestsBase
    {
        [TestMethod]
        public async Task CreateApplicationUserAsync_Test()
        {
            var dbContext = base.BuildFairPlaySocialDatabaseContext();
            dbContext.ApplicationUser.RemoveRange();
            await dbContext.SaveChangesAsync(base._cancellationToken);
            var loggerFactory = 
            LoggerFactory.Create(configure => 
            {
            });
            var logger = loggerFactory.CreateLogger<ApplicationUserService>();
            ApplicationUserService applicationUserService = new(dbContext, logger);
            DataAccess.Models.ApplicationUser testUser = new DataAccess.Models.ApplicationUser()
            {
                AzureAdB2cobjectId = Guid.NewGuid(),
                EmailAddress = "tests@test.test",
                FullName = "Test User",
                LastLogIn = DateTimeOffset.UtcNow
            };
            await applicationUserService.CreateApplicationUserAsync(
                testUser, base._cancellationToken);
            var createdUser = await dbContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.EmailAddress == testUser.EmailAddress);
            Assert.IsNotNull(createdUser);
        }
    }
}
