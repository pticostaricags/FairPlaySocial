using Microsoft.Playwright;

namespace FairPlaySocial.AutomatedPostDeploymentTests.Firefox
{
    [TestClass]
    public class NavigateToRoot_Tests : PostDeploymentTestsBase
    {
        [TestMethod]
        public async Task Test_LoginFlowAsync()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Timeout = (float)TimeSpan.FromSeconds(60).TotalMilliseconds
            });
            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();

            page.Context.SetDefaultNavigationTimeout((float)TimeSpan.FromSeconds(90).TotalMilliseconds);
            page.Context.SetDefaultTimeout((float)TimeSpan.FromSeconds(90).TotalMilliseconds);
            
            await page.GotoAsync(BaseUrl);

            await page.GetByRole(AriaRole.Button, new() { NameString = "Log In" }).ClickAsync();

            await page.GetByPlaceholder("Email Address").ClickAsync();

            await page.GetByPlaceholder("Email Address").FillAsync(TestAzureAdB2CAuthConfiguration!.UserRoleUsername!);

            await page.GetByPlaceholder("Email Address").PressAsync("Tab");

            await page.GetByPlaceholder("Password").FillAsync(TestAzureAdB2CAuthConfiguration!.UserRolePassword!);

            await page.GetByRole(AriaRole.Button, new() { NameString = "Sign in" }).ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Log Out" }).ClickAsync();
            var loggedoutIndicatorLocator = page.GetByRole(AriaRole.Button, new() { NameString = "Log In" }); ;
            await Assertions.Expect(loggedoutIndicatorLocator).ToHaveTextAsync("Log In",
                options:new LocatorAssertionsToHaveTextOptions() 
                {
                    Timeout = (float)TimeSpan.FromSeconds(90).TotalMilliseconds
                });
        }

        [TestMethod]
        public async Task Test_NavigateToHomeAsync()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });
            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();

            page.Context.SetDefaultNavigationTimeout((float)TimeSpan.FromSeconds(90).TotalMilliseconds);
            page.Context.SetDefaultTimeout((float)TimeSpan.FromSeconds(90).TotalMilliseconds);

            await page.GotoAsync(BaseUrl);

            await page.GetByText("FairPlaySocial (Beta)").ClickAsync();

            await page.GetByText("Welcome to FairPlaySocial. The Multi-platform system to share your thoughts.").ClickAsync();

            await page.Locator("a").First.ClickAsync();
        }
    }
}