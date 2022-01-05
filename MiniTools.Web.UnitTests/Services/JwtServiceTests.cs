using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniTools.Web.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiniTools.Web.Services.Tests;

[TestClass()]
public class JwtServiceTests
{
    Mock<IConfiguration> mockConfiguration = new Mock<IConfiguration>();
    Mock<IOptionsMonitor<JwtSettings>> mockOptionsMonitor = new Mock<IOptionsMonitor<JwtSettings>>();

    [TestInitialize]
    public void BeforeTest()
    {
        mockConfiguration = new Mock<IConfiguration>();
        mockOptionsMonitor = new Mock<IOptionsMonitor<JwtSettings>>();
        mockOptionsMonitor.Setup(m => m.Get("jwt")).Returns(new JwtSettings
        {
            SecretKey = "someSecretKey123",
            ValidAudience = "someValidAudience",
            ValidIssuer = "someValidIssuer"
        });
    }

    [TestMethod()]
    public void JwtServiceTest()
    {
        JwtService jwtService = new JwtService();

        Assert.IsNotNull(jwtService);
    }

    [TestMethod()]
    public void JwtServiceTest1()
    {
        JwtService jwtService = new JwtService(mockConfiguration.Object, mockOptionsMonitor.Object);

        Assert.IsNotNull(jwtService);
    }

    [TestMethod()]
    public void CreateTokenTest()
    {
        JwtService jwtService = new JwtService(mockConfiguration.Object, mockOptionsMonitor.Object);

        var result = jwtService.CreateToken(new List<Claim>());

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(JwtSecurityToken));
    }

    [TestMethod()]
    public void ToCompactSerializationFormatTest()
    {
        JwtService jwtService = new JwtService(mockConfiguration.Object, mockOptionsMonitor.Object);

        var token = jwtService.CreateToken(new List<Claim>());

        var result = jwtService.ToCompactSerializationFormat(token);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod()]
    public void GetClaimsTest()
    {
        JwtService jwtService = new JwtService(mockConfiguration.Object, mockOptionsMonitor.Object);

        var token = jwtService.CreateToken(new List<Claim>
            {
                new Claim(ClaimTypes.Name, "someName")
            });

        var jwt = jwtService.ToCompactSerializationFormat(token);

        var result = jwtService.GetClaims(jwt);

        Assert.IsNotNull(result);
    }

    [TestMethod()]
    public void GetClaimsMalFormJwtTest()
    {
        JwtService jwtService = new JwtService(mockConfiguration.Object, mockOptionsMonitor.Object);
        Assert.ThrowsException<ArgumentException>(() =>
           jwtService.GetClaims("someMalformJwt")
        );
    }
}
