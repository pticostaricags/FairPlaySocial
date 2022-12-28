using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Enums;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace FairPlaySocial.AutomatedTests.ClientServices
{
    [TestClass]
    public class MyPostClientService_Tests : ClientServicesTestsBase
    {

        [TestMethod]
        public async Task Test_CreateMyPostAsync_NotAllowed()
        {
            CreateTestUsers();
            await base.SignIn(Role.User);
            var dbContext = ClientServicesTestsBase.GetDbContextInstance();
            var myUser = await dbContext.ApplicationUser
                .SingleAsync(p => p.AzureAdB2cobjectId.ToString() ==
            ClientServicesTestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId);
            var otherUser = await dbContext.ApplicationUser
                .FirstAsync(p => p.AzureAdB2cobjectId.ToString() !=
            ClientServicesTestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId);
            var myPost = new Post()
            {
                OwnerApplicationUserId = myUser.ApplicationUserId,
                PostTypeId = (byte)Common.Enums.PostType.Post,
                PostVisibilityId = (short)Common.Enums.PostVisibility.Public,
                Text = "My Post Text"
            };
            await dbContext.Post.AddAsync(myPost);
            await dbContext.SaveChangesAsync();

            var otherUserPost = new Post()
            {
                OwnerApplicationUserId = otherUser.ApplicationUserId,
                PostTypeId = (byte)Common.Enums.PostType.Post,
                PostVisibilityId = (short)Common.Enums.PostVisibility.Public,
                Text = "Other User Post Text"
            };

            await dbContext.Post.AddAsync(otherUserPost);
            await dbContext.SaveChangesAsync();

            MyPostClientService myPostClientService = base.CreateMyPostClientService();
            try
            {
                await myPostClientService.DeleteMyPostAsync(otherUserPost.PostId, CancellationToken.None);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Deleting other users posts is not allowed"))
                    Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task Test_CreateMyPostAsync_Allowed()
        {
            await base.SignIn(Role.User);
            var myPostClientService = base.CreateMyPostClientService();
            string postText = $"Automated Test Post Created at : {DateTimeOffset.UtcNow}";
            var imageStreamFullName = "FairPlaySocial.AutomatedTests.Resources.Images.TestImage1.jpg";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageStreamFullName);
            MemoryStream memoryStream = new MemoryStream();
            await stream!.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            await myPostClientService.CreateMyPostAsync(createPostModel: new()
            {
                Photo = new()
                {
                    AlternativeText = "Test Image",
                    Filename = "TestImage1",
                    ImageBytes = bytes,
                    ImageType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                },
                PostVisibilityId = (short)Common.Enums.PostVisibility.Public,
                Text = postText,
                Tag1 = "Tag1",
                Tag2 = "Tag2",
                Tag3 = "Tag3"
            }, CancellationToken.None);
            var myFeedClientService = base.CreateMyFeedClientService();
            var myHomeFeed = await myFeedClientService.GetMyHomeFeedAsync(pageRequestModel: new()
            {
                PageNumber = 1
            }, CancellationToken.None);
            Assert.IsNotNull(myHomeFeed);
            Assert.IsTrue(myHomeFeed.Items!.Length == 1);
        }

        private static void CreateTestUsers()
        {
            var dbContext = ClientServicesTestsBase.GetDbContextInstance();

            var userRole = dbContext.ApplicationRole.Single(p => p.Name == Constants.Roles.User);
            for (int i = 0; i < 10; i++)
            {
                DataAccess.Models.ApplicationUser entity = new()
                {
                    AzureAdB2cobjectId = Guid.NewGuid(),
                    EmailAddress = $"Test-{i}@test.test",
                    FullName = $"Test User {i}",
                    LastLogIn = DateTimeOffset.UtcNow,
                };
                dbContext.ApplicationUser.Add(entity);
                dbContext.SaveChanges(true);
                dbContext.ApplicationUserRole.Add(new()
                {
                    ApplicationUserId = entity.ApplicationUserId,
                    ApplicationRoleId = userRole.ApplicationRoleId
                });
                dbContext.SaveChanges(true);
            }
        }
    }
}
