using FairPlaySocial.ClientServices;
using FairPlaySocial.Common.Enums;
using FairPlaySocial.Common.Global;
using FairPlaySocial.DataAccess.Data;
using FairPlaySocial.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Threading;

namespace FairPlaySocial.AutomatedTests.ClientServices
{
    [TestClass]
    public class MyPostClientService_Tests : ClientServicesTestsBase
    {
        [TestInitialize]
        public async Task Initialize()
        {
            await CleanTestsData();
        }

        [TestCleanup()]
        public async Task Cleanup()
        {
            await CleanTestsData();
        }

        private async Task CleanTestsData()
        {
            var dbContext = ClientServicesTestsBase.GetDbContextInstance();
            var allUnitTestPosts = await dbContext.Post
                .Include(p=>p.PostTag)
                .Include(p=>p.Photo)
                .Include(p=>p.PostUrl)
                .Include(p=>p.InverseReplyToPost)
                .Where(p => p.Text.Contains("Automated Test Post"))
                .Select(p=>p.PostId).ToArrayAsync();
            if (allUnitTestPosts?.Length > 0)
            {
                foreach (var singlePostId in allUnitTestPosts) 
                {
                    var postEntity = await dbContext.Post.Include(p => p.Photo)
                        .AsNoTracking()
                        .Where(p => p.PostId == singlePostId).SingleAsync();
                    var executionStrategy = dbContext.Database.CreateExecutionStrategy();
                    await executionStrategy.ExecuteAsync(async () =>
                    {
                        await using var transaction = await dbContext.Database
                        .BeginTransactionAsync();
                        var deletedPostKeyPhrases =
                        await dbContext
                        .PostKeyPhrase
                        .Where(p => p.PostId == singlePostId)
                        .ExecuteDeleteAsync();
                        var deletedDislikedPosts =
                        await dbContext
                        .DislikedPost
                        .Where(p => p.PostId == singlePostId)
                        .ExecuteDeleteAsync();
                        var deletedlikedPosts =
                            await dbContext
                            .LikedPost
                        .Where(p => p.PostId == singlePostId)
                            .ExecuteDeleteAsync();
                        var deletedTags =
                            await dbContext.PostTag.Where(p => p.PostId == singlePostId)
                            .ExecuteDeleteAsync();
                        var deletedPostUrls =
                        await dbContext.PostUrl.Where(p => p.PostId == singlePostId)
                        .ExecuteDeleteAsync();
                        var deletedPostReplies =
                        await dbContext.Post.Where(p => p.RootPostId == singlePostId)
                        .OrderByDescending(p => p.PostId)
                        .ExecuteDeleteAsync();
                        var deletedPosts =
                        await dbContext.Post.Where(p => p.PostId == singlePostId)
                        .ExecuteDeleteAsync();
                        var deletedPhotos =
                        await dbContext.Photo
                        .Where(p => p.PhotoId == postEntity.PhotoId)
                        .ExecuteDeleteAsync();
                        await transaction.CommitAsync();
                    });
                }
                await dbContext.SaveChangesAsync();
            }
        }

        [TestMethod]
        public async Task Test_DeleteMyPostAsync_Allowed()
        {
            await base.SignIn(Role.User);
            var dbContext = ClientServicesTestsBase.GetDbContextInstance();
            var myUser = await dbContext.ApplicationUser
                .SingleAsync(p => p.AzureAdB2cobjectId.ToString() ==
            ClientServicesTestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId);
            byte[] bytes = await GetTestImageBytes();
            var myPost = new Post()
            {
                Photo = new()
                {
                    AlternativeText = "Test Image",
                    Filename = "TestImage1",
                    ImageBytes = bytes,
                    ImageType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                },
                OwnerApplicationUserId = myUser.ApplicationUserId,
                PostTypeId = (byte)Common.Enums.PostType.Post,
                PostVisibilityId = (short)Common.Enums.PostVisibility.Public,
                Text = "Automated Test Post"
            };
            await dbContext.Post.AddAsync(myPost);
            await dbContext.SaveChangesAsync();

            MyPostClientService myPostClientService = base.CreateMyPostClientService();
            try
            {
                await myPostClientService.DeleteMyPostAsync(myPost.PostId, CancellationToken.None);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Deleting other users posts is not allowed"))
                    Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task Test_DeleteMyPostAsync_NotAllowed()
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
            byte[] bytes = await GetTestImageBytes();
            var myPost = new Post()
            {
                Photo = new()
                {
                    AlternativeText = "Test Image",
                    Filename = "TestImage1",
                    ImageBytes = bytes,
                    ImageType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                },
                OwnerApplicationUserId = myUser.ApplicationUserId,
                PostTypeId = (byte)Common.Enums.PostType.Post,
                PostVisibilityId = (short)Common.Enums.PostVisibility.Public,
                Text = "Automated Test Post"
            };
            await dbContext.Post.AddAsync(myPost);
            await dbContext.SaveChangesAsync();

            var otherUserPost = new Post()
            {
                Photo = new()
                {
                    AlternativeText = "Test Image",
                    Filename = "TestImage1",
                    ImageBytes = bytes,
                    ImageType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                },
                OwnerApplicationUserId = otherUser.ApplicationUserId,
                PostTypeId = (byte)Common.Enums.PostType.Post,
                PostVisibilityId = (short)Common.Enums.PostVisibility.Public,
                Text = "Automated Test Post"
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
        public async Task Test_CreateMyPostAsync()
        {
            await base.SignIn(Role.User);
            var myPostClientService = base.CreateMyPostClientService();
            string postText = $"Automated Test Post Created at : {DateTimeOffset.UtcNow}";
            byte[] bytes = await GetTestImageBytes();
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
            Assert.IsNotNull(myHomeFeed.Items!.Where(p => p.Text == postText).Any());
        }

        private static async Task<byte[]> GetTestImageBytes()
        {
            var imageStreamFullName = "FairPlaySocial.AutomatedTests.Resources.Images.TestImage1.jpg";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageStreamFullName);
            MemoryStream memoryStream = new();
            await stream!.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return bytes;
        }

        private static void CreateTestUsers()
        {
            var dbContext = ClientServicesTestsBase.GetDbContextInstance();

            var userRole = dbContext.ApplicationRole.Single(p => p.Name == Constants.Roles.User);
            for (int i = 0; i < 10; i++)
            {
                string userEmailAddress = $"Test-{i}@test.test";
                if (!dbContext.ApplicationUser.Any(p => p.EmailAddress == userEmailAddress))
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
}
