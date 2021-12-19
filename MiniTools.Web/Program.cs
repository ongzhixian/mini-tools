using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.OpenApi.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using AppStartup = MiniTools.Web.Services.AppStartupService;
using MiniTools.Web.ActionFilters;
using MiniTools.Web.Health;
using MiniTools.Web.Services;
using MongoDB.Driver;
using MiniTools.Web.Options;

var builder = WebApplication.CreateBuilder(args);

AppStartup.AddAppSettings(builder.Configuration, builder.Environment);

AppStartup.SetupLogging(builder.Host);

AppStartup.SetupHttpLogging(builder.Configuration, builder.Services);

AppStartup.SetupAuthentication(builder.Configuration, builder.Services);

AppStartup.SetupAuthorization(builder.Services);

AppStartup.SetupHttpClient(builder.Services);

AppStartup.SetupSession(builder.Services);

AppStartup.SetupAntiForgery(builder.Services);

AppStartup.SetupCookies(builder.Services);

AppStartup.SetupCors(builder.Services);

AppStartup.SetupSwagger(builder.Configuration, builder.Services);

// Add services to the container.

//builder.Services.AddHealthChecks();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

// builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // Only if using EF

builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews(options => {
    // Add the filters that you want to apply globally here
    //options.Filters.Add(new LogActionFilterAttribute());

    // options.Filters.Add(
    //     new Microsoft.AspNetCore.Mvc.ServiceFilterAttribute(
    //         typeof(MiniTools.Web.Filters.LogResultFilterService)
    //     ));

    // options.Filters.Add(
    //     new Microsoft.AspNetCore.Mvc.ServiceFilterAttribute(
    //         typeof(MiniTools.Web.Filters.LogActionFilterService)
    //     ));
});

builder.Services.AddHealthChecks()
    .AddCheck<SampleHealthCheck>("Sample")
    .AddCheck<SampleHealthCheck2>("Sample2");

// ...Start adding your services here...

builder.Services.AddScoped<MiniTools.Web.Filters.LogResultFilterService>();
builder.Services.AddScoped<MiniTools.Web.Filters.LogActionFilterService>();
builder.Services.AddScoped<IMongoClient, MongoClient>();
builder.Services.AddScoped<ILoginService, LoginService>();

// builder.Services.AddOptions<MongoDbOptions>("ass");

// services.Configure<NormalThemeDashboardSettings>(_configuration.GetSection("DashboardThemeSettings:NormalTheme"));  
// services.Configure<DarkThemeDashboardSettings>(_configuration.GetSection("DashboardThemeSettings:DarkTheme"));  

// services.AddSingleton<IThemeConfigurationReader, ThemeConfigurationReader>();  

// services.Configure<DashboardThemeSettings>("Normal", _configuration.GetSection("DashboardThemeSettings:NormalTheme"));  
// services.Configure<DashboardThemeSettings>("Dark", _configuration.GetSection("DashboardThemeSettings:DarkTheme"));  


// A couple way to configure
Console.WriteLine("QQQQQQQQQQQ {0}", builder.Configuration["mongodb:minitools:ConnectionString"]);

// builder.Services.Configure<MongoDbSettings>(
//     "mongodb:minitools", (s) => {
//         s.ConnectionString = "ffake"; 
//         //builder.Configuration["mongodb:minitools:ConnectionString"];
//     });

builder.Services.Configure<MongoDbSettings>(
    "mongodb:minitools", 
    builder.Configuration.GetSection("mongodb:minitools")
    );


// ...End of adding your services here...

// Some ad-hoc development logging
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Is Development");
    Console.WriteLine("Jwt:ValidAudience    [{0}]", builder.Configuration["Jwt:ValidAudience"]);
    Console.WriteLine("Jwt:ValidIssuer      [{0}]", builder.Configuration["Jwt:ValidIssuer"]);
    Console.WriteLine("Jwt:SecretKey        [{0}]", builder.Configuration["Jwt:SecretKey"]);
}


var app = builder.Build();


// Configure the HTTP request pipeline.
// The correct middleware order should follow:
// ExceptionHandler
// HSTS --
// HttpsRedirection
// Static Files
// Routing
// CORS
// Authentication
// Authorization

// Development-only 
if (app.Environment.IsDevelopment())
{
    // Logging sample code:
    // Method 1: app.Logger
    //app.Logger.LogInformation("Your custom log message");
    // Method 2: Get Logger via DI
    //ILogger? logger = app.Services.GetService<ILogger<Program>>() ?? throw new Exception($"Service ({nameof(ILogger<Program>)} does not exist.");
    //logger.LogInformation("Your custom log message");

    //if (app.Configuration.GetValue<bool>("Application:EnableHttpLogging"))
    //    app.UseHttpLogging();

    //app.UseSwagger();
    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{openApiInfo.Title}; {openApiInfo.Version}");
    //});
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    //app.UseDatabaseErrorPage
}
else
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// TODO: customise this part
// app.UseStatusCodePages();
// app.UseStatusCodePages(async statusCodeContext =>
// {
//     // using static System.Net.Mime.MediaTypeNames;
//     statusCodeContext.HttpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
//     The default message looks like `Status Code: 404; Not Found`
//     await statusCodeContext.HttpContext.Response.WriteAsync(
//         $"XXX Status Code Page: {statusCodeContext.HttpContext.Response.StatusCode}");
// });
// https://localhost:7001/Home/StatusCode/404
// app.UseStatusCodePagesWithRedirects("/http-status/{0}");
app.UseStatusCodePagesWithReExecute("/http-status/{0}");

app.UseStaticFiles();

// app.UseCookiePolicy();

app.UseRouting();

// app.UseRequestLocalization();

app.UseCors("DebugAllowAll");

app.UseAuthentication();

// app.UseAuthorization();

app.UseSession();

// app.UseResponseCompression();

// app.UseResponseCaching();

app.MapHealthChecks("/health").AllowAnonymous();

//app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Other development-only middleware
if (app.Environment.IsDevelopment())
{
    // Custom logging
    // Logging sample code:
    // Method 1: app.Logger
    //app.Logger.LogInformation("Your custom log message");
    // Method 2: Get Logger via DI
    //ILogger? logger = app.Services.GetService<ILogger<Program>>() ?? throw new Exception($"Service ({nameof(ILogger<Program>)} does not exist.");
    //logger.LogInformation("Your custom log message");

    // Feature: HTTP Logging
    //if (app.Configuration.GetValue<bool>("Application:EnableHttpLogging"))
    //    app.UseHttpLogging();


    // Feature: Swagger middleware
    //app.UseSwagger();
    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{openApiInfo.Title}; {openApiInfo.Version}");
    //});


    // Feature: List all routes(?)...not working...
    // Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider actionProvider = app.Services.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>();
    // log.Log(LogLevel.Information, "Available routes:");
    // var routes = actionProvider.ActionDescriptors.Items.Where(x => x.AttributeRouteInfo != null);
    // foreach (var route in routes)
    // {
    //     log.Log(LogLevel.Information, $"{route.AttributeRouteInfo.Template}");
    // }

}

app.Run();
