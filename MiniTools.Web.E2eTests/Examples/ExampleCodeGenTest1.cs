using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MiniTools.Web.E2eTests;

[TestCategory("e2e")]
[TestClass]
public class ExampleCodeGenTest1
{
    [TestMethod]
    public async Task TestMethod1Async()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            Channel = "msedge",
        });
        var context = await browser.NewContextAsync();

        // Open new page
        var page = await context.NewPageAsync();

        if (page != null)
        {
            int result = await page.EvaluateAsync<int>("() => 7 * 3");
            Assert.AreEqual(21, result);
        }

        // 
        //// Go to https://localhost:7241/
        //await page.GotoAsync("https://localhost:7241/");
        //// Click main[role="main"] >> text=Login
        //await page.ClickAsync("main[role=\"main\"] >> text=Login");
        //// Assert.AreEqual("https://localhost:7241/Login", page.Url);

        //// Click input[name="Password"]
        //await page.ClickAsync("input[name=\"Password\"]");
        //// Fill input[name="Password"]
        //await page.FillAsync("input[name=\"Password\"]", "ASds");
        //// Click text=Submit
        //await page.ClickAsync("text=Submit");
        //// Assert.AreEqual("https://localhost:7241/Login", page.Url);
    }
}