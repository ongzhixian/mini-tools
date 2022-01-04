using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MiniTools.Web.E2eTests;


[TestCategory("e2e")]
[TestClass]
public class ExamplePageTest1 : PageTest
{
    [TestMethod]
    public async Task ShouldAdd()
    {
        if (Page != null)
        {
            int result = await Page.EvaluateAsync<int>("() => 7 + 3");
            Assert.AreEqual(10, result);
        }
    }

    [TestMethod]
    public async Task ShouldMultiply()
    {
        if (Page != null)
        {
            int result = await Page.EvaluateAsync<int>("() => 7 * 3");
            Assert.AreEqual(21, result);
        }
    }
}
