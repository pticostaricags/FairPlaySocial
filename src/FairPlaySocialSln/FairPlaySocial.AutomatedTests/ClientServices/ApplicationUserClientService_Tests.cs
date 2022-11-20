using FairPlaySocial.Common.Global;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedTests.ClientServices
{
    [TestClass]
    public class ApplicationUserClientService_Tests : ClientServicesTestsBase
    {
        [TestMethod]
        public async Task Test_GetMyRolesAsync()
        {
            await base.SignIn(Role.User);
            var applicationUserClientService = CreateApplicationUserClientService();
            var roles = await applicationUserClientService.GetMyRolesAsync(CancellationToken.None);
            Assert.IsNotNull(roles);
            Assert.AreEqual<string>(roles[0], Constants.Roles.User);
        }

        [TestMethod]
        public async Task Test_CreateApplicationUserAsync_Unauthorized()
        {
            await base.SignIn(Role.User);
            var applicationUserClientService = base.CreateApplicationUserClientService();
            var exception = 
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => 
            {
                await applicationUserClientService.CreateApplicationUserAsync(
                    model: new Models.ApplicationUser.CreateApplicationUserModel()
                    {
                        AzureAdB2cobjectId= Guid.NewGuid(),
                        EmailAddress="test@test.test",
                        FullName="Automated Test",
                        LastLogIn = DateTimeOffset.UtcNow
                    }, CancellationToken.None);
            });
            Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, exception.StatusCode);
        }
    }
}
