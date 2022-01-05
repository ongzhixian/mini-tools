using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Playwright;

namespace MiniTools.Web.E2eTests;

[TestCategory("e2e")]
[TestClass]
public class LoginPageTest : PageTest
{
    [TestMethod]
    public async Task LoginWithValidCredentials()
    {
        if (Context == null)
            return;

        var page = await Context.NewPageAsync();
        await page.GotoAsync("https://localhost:7001/Login");

        await page.ClickAsync("input[name=\"Username\"]");
        await page.FillAsync("input[name=\"Username\"]", "dev");
        await page.PressAsync("input[name=\"Username\"]", "Tab");
        await page.FillAsync("input[name=\"Password\"]", "dev");
        await page.ClickAsync("button:has-text(\"Log in\")");

        // Grab all cookies
        var allCookies = await Context.CookiesAsync();

        // i should have authentication cookie ("Cookie2")
        var authCookie = allCookies.FirstOrDefault(r => r.Name == "Cookie2");

        // i should have an a.nav-link with text "Log out"
        IReadOnlyList<IElementHandle>? queryResults = await page.QuerySelectorAllAsync("a.nav-link:text(\"Log out\")");

        Assert.IsNotNull(authCookie);
        Assert.AreEqual(1, queryResults.Count);
    }
}
