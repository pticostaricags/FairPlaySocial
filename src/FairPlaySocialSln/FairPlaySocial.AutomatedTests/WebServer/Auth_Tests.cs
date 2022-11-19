using FairPlaySocial.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedTests.Website
{
    [TestClass]
    public class Auth_Tests : WebServerTestsBase
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
    }
}
