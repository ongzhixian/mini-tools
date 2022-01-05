using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniTools.Web.UnitTests.Helpers;
internal class MockHelper
{
    public static DefaultHttpContext MakeHttpContext()
    {
        // Need mock of IAuthenticationService to handle HttpContext.SignIn
        var mockAuthenticationService = new Mock<IAuthenticationService>();

        mockAuthenticationService
            .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        mockAuthenticationService
            .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        // Need mock of IUrlHelperFactory to properly handle RedirectToAction
        var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();

        // Need mock of ITempDataProvider to handle TempData
        TempDataDictionaryFactory tempDataDictionaryFactory = new TempDataDictionaryFactory(new Mock<ITempDataProvider>().Object);

        // Setup mock service provider

        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(mockAuthenticationService.Object);

        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IUrlHelperFactory)))
            .Returns(mockUrlHelperFactory.Object);

        serviceProviderMock
            .Setup(_ => _.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns(tempDataDictionaryFactory);

        return new DefaultHttpContext()
        {
            RequestServices = serviceProviderMock.Object
        };
    }
}
