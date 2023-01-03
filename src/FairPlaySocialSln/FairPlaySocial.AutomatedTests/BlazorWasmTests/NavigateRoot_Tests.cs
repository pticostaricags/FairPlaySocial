using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.AutomatedTests.BlazorWasmTests
{
    [TestClass]
    public class NavigateRoot_Tests: BlazorWasmTestsBase
    {
        [TestMethod]
        public async Task Test_SiteIsLoaded()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions()
                {
                    Headless=false
                });
            var page = await browser.NewPageAsync();
            await page.WaitForResponseAsync(base.RootUri.AbsoluteUri);
            
        }

        
    }
}
