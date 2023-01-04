using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace FairPlaySocial.AutomatedPostDeploymentTests.Chromium
{
    [TestClass]
    public class NavigateToRoot_Tests : PostDeploymentTestsBase
    {
        [TestMethod]
        public async Task Test_LoginFlowAsync()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });
            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();

            await page.GotoAsync(BaseUrl);

            await page.GetByRole(AriaRole.Button, new() { NameString = "Log In" }).ClickAsync();

            await page.GetByPlaceholder("Email Address").ClickAsync();

            await page.GetByPlaceholder("Email Address").FillAsync(TestAzureAdB2CAuthConfiguration!.UserRoleUsername!);

            await page.GetByPlaceholder("Email Address").PressAsync("Tab");

            await page.GetByPlaceholder("Password").FillAsync(TestAzureAdB2CAuthConfiguration!.UserRolePassword!);

            await page.GetByRole(AriaRole.Button, new() { NameString = "Sign in" }).ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Log Out"}).ClickAsync();
            var loggedoutIndicatorLocator = page.GetByRole(AriaRole.Heading, new() { NameString = "Log Out Succeeded" });;
            await Assertions.Expect(loggedoutIndicatorLocator).ToHaveTextAsync("Log Out Succeeded");
        }

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