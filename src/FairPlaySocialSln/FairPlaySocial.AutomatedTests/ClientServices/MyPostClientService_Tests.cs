using FairPlaySocial.Common.Enums;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using System.Reflection;

namespace FairPlaySocial.AutomatedTests.ClientServices
{
    [TestClass]
    public class MyPostClientService_Tests : ClientServicesTestsBase
    {
        [TestMethod]
        public async Task Test_CreateMyPostAsync_Allowed()
        {
            await base.SignIn(Role.User);
            var myPostClientService = base.CreateMyPostClientService();
            string postText = $"Automated Test Post Created at : {DateTimeOffset.UtcNow}";
            var imageStreamFullName = "FairPlaySocial.AutomatedTests.Resources.Images.TestImage1.jpg";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageStreamFullName);
            MemoryStream memoryStream= new MemoryStream();
            await stream!.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            await myPostClientService.CreateMyPostAsync(createPostModel: new()
            {
                Photo = new() 
                {
                    AlternativeText="Test Image",
                    Filename ="TestImage1",
                    ImageBytes=bytes,
                    ImageType=System.Net.Mime.MediaTypeNames.Image.Jpeg
                },
                PostVisibilityId = (short)PostVisibility.Public,
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
    }
}
