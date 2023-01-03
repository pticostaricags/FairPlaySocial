using Microsoft.Playwright;

namespace FairPlaySocial.AutomatedPostDeploymentTests.Chromium
{
    [TestClass]
    public class NavigateToRoot_Tests : PostDeploymentTestsBase
    {
        [TestMethod]
        public async Task Test_NavigateToHomeAsync()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });
            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();

            await page.GotoAsync(BaseUrl);

            await page.GetByText("FairPlaySocial (Beta)").ClickAsync();

            await page.GetByText("Welcome to FairPlaySocial. The Multi-platform system to share your thoughts.").ClickAsync();

            await page.Locator("a").First.ClickAsync();
        }
    }
}