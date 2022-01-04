using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;

namespace MiniTools.Web.E2eTests;

[TestCategory("e2e")]
[TestClass]
public class LoginPageTest : PageTest
{
    [TestMethod]
    public async Task LoginWithValidCredentials()
    {
        //Context.NewPageAsync
        //if (Page != null)
        //{
        //    int result = await Page.EvaluateAsync<int>("() => 7 + 3");
        //    Assert.AreEqual(10, result);
        //}

        //using var playwright = await Playwright.CreateAsync();
        //await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        //{
        //    Headless = false,
        //});
        //var context = await browser.NewContextAsync();

        if (Context == null)
            return;

        var page = await Context.NewPageAsync();
        await page.GotoAsync("https://localhost:7001/Login");
        await page.ClickAsync("input[name=\"Username\"]");
        await page.FillAsync("input[name=\"Username\"]", "dev");
        await page.PressAsync("input[name=\"Username\"]", "Tab");
        await page.FillAsync("input[name=\"Password\"]", "dev");
        await page.ClickAsync("button:has-text(\"Log in\")");


        var allCookies = await Context.CookiesAsync();

        var fakeCookie = allCookies.FirstOrDefault(r => r.Name == "Cookie1");
        var authCookie = allCookies.FirstOrDefault(r => r.Name == "Cookie2");

        //await page.ClickAsync("text=Log in");
        //await page.Contexts[0].CookiesAsync();


        //System.Collections.Generic.IReadOnlyList<Microsoft.Playwright.BrowserContextCookiesResult>? cookies = await page.Context.CookiesAsync();
        //var x = await Context.CookiesAsync();

        //// Go to https://localhost:7001/Login
        //await page.GotoAsync("https://localhost:7001/Login");

        //// Go to https://localhost:7001/Login
        //await page.GotoAsync("https://localhost:7001/Login");

        //// Click text=Log out
        //await page.ClickAsync("text=Log out");
        //// Assert.AreEqual("https://localhost:7001/", page.Url);

        //// Click text=© 2021 - Mini-Tools v1.0.0 - Privacy (False)
        //await page.ClickAsync("text=© 2021 - Mini-Tools v1.0.0 - Privacy (False)");
        Assert.IsNotNull(authCookie);
    }


}
