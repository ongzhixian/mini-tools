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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using MiniTools.Web.MongoEntities;
using MiniTools.Web.DataEntities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MiniTools.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// TODO: Move configuration stuff here

AppStartup.AddAppSettings(builder.Configuration, builder.Environment);

AppStartup.SetupLogging(builder.Configuration, builder.Logging, builder.Host);

AppStartup.SetupHttpLogging(builder.Configuration, builder.Services);

AppStartup.SetupAuthentication(builder.Configuration, builder.Services);

AppStartup.SetupAuthorization(builder.Services);

AppStartup.SetupHttpClient(builder.Configuration, builder.Services);

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

builder.Services.AddControllersWithViews(options =>
{
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
// builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserCollectionService, UserCollectionService>();
builder.Services.AddScoped<IJwtService, JwtService>();

//builder.Services.AddTransient<MiniTools.Web.Helpers.ActivityEnricher>(sp =>
//{
//    var httpAccessor = sp.GetRequiredService<IHttpContextAccessor>();
//    return new MiniTools.Web.Helpers.ActivityEnricher(httpAccessor);
//});



//builder.Services.AddHttpClient<AuthenticationService>();
builder.Services.AddHttpClient<UserApiService>();
builder.Services.AddHttpClient<IAuthenticationApiService, AuthenticationApiService>();



// builder.Services.AddOptions<MongoDbOptions>("ass");

// services.Configure<NormalThemeDashboardSettings>(_configuration.GetSection("DashboardThemeSettings:NormalTheme"));  
// services.Configure<DarkThemeDashboardSettings>(_configuration.GetSection("DashboardThemeSettings:DarkTheme"));  

// services.AddSingleton<IThemeConfigurationReader, ThemeConfigurationReader>();  

// services.Configure<DashboardThemeSettings>("Normal", _configuration.GetSection("DashboardThemeSettings:NormalTheme"));  
// services.Configure<DashboardThemeSettings>("Dark", _configuration.GetSection("DashboardThemeSettings:DarkTheme"));  

// MongoDb

string miniToolsConnectionString = builder.Configuration.GetValue<string>("mongodb:minitools:ConnectionString");


// Instead of placing a `[BsonElement("some-mongoDb-name")]` attribute name,
// we can do class mapping
//if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
//{
//    BsonClassMap.RegisterClassMap<User>(c =>
//    {
//        c.AutoMap(); // Required!

//        //c.GetMemberMap<string>(r => r.Note).SetElementName("someNote"); // Works
//        //c.GetMemberMap(r => r.Note).SetElementName("someNote2"); // Works

//        // No effect; Id cannot be mapped
//        //c.GetMemberMap(r => r.Id).SetElementName("someId"); 

//        // Not work (runtime error); Because these fields are inherited
//        // To get it work, we need to map fields of inherited class

//        //c.GetMemberMap(r => r.Username).SetElementName("username");
//        //c.GetMemberMap(r => r.Password).SetElementName("password");
//        //c.GetMemberMap(r => r.Status).SetElementName("status");
//        //c.GetMemberMap(r => r.DateCreated).SetElementName("dateCreated");
//        //c.GetMemberMap(r => r.PasswordExpiryDate).SetElementName("passwordExpiryDate");
//    });
//}

// Ok, forget about class mapping
// We can tweak the default MongoDb conventions.
// See: https://mongodb.github.io/mongo-csharp-driver/1.11/serialization/#conventions
//var conventionPack = new ConventionPack();
//conventionPack.Add(new CamelCaseElementNameConvention());
// --OR-- one-liner (same thing)
//var conventionPack = new ConventionPack { new CamelCaseElementNameConvention()};
//ConventionRegistry.Register("camelCase", conventionPack, t => true);

// We can also filter which classes to apply the conventions
//ConventionRegistry.Register(
//   "My Custom Conventions",
//   conventionPack,
//   t => t.FullName.StartsWith("MyNamespace."));


//builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(miniToolsConnectionString));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    //IMongoClient mongoClient = sp.GetRequiredService<IMongoClient>();
    IMongoClient mongoClient = new MongoClient(miniToolsConnectionString);
    string databaseName = MongoUrl.Create(miniToolsConnectionString).DatabaseName;
    return mongoClient.GetDatabase(databaseName);
});




builder.Services.AddSingleton<IMongoCollection<User>>(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<User>("user"));

// Configurations

builder.Services.AddSingleton<ApplicationSettings>(sp => new ApplicationSettings(builder.Configuration.GetSection("application")));

// A couple way to configure

// Console.WriteLine("QQQQQQQQQQQ {0}", builder.Configuration["mongodb:minitools:ConnectionString"]);

// builder.Services.Configure<MongoDbSettings>(
//     "mongodb:minitools", (s) => {
//         s.ConnectionString = "ffake"; 
//         //builder.Configuration["mongodb:minitools:ConnectionString"];
//     });

// builder.Services.Configure<MongoDbSettings>(options => builder.Configuration.GetSection("mongodb:minitools").Bind(options));

builder.Services.Configure<MongoDbSettings>(
    "mongodb:minitools",
    builder.Configuration.GetSection("mongodb:minitools")
    );



// Original ApiSettings use `Api` as property to hold Dictionary<string, string>
// So using the following code does not work; cannot map `Api` directly to Dictionary<string, string> type
// builder.Services.Configure<ApiSettings>(
//     "api", 
//     builder.Configuration.GetSection("Api")
//     );

// To use `Api` as property to hold Dictionary<string, string>,
// we need to use one of following methods:

// Method 1: Simple assignment to `Api` property
//builder.Services.Configure<ApiSettings>(
//    "api", 
//    apiSettings => apiSettings.Api = builder.Configuration.GetSection("Api").Get<Dictionary<string, string>>()
//    );

// Method 2: If we have an more elaborate class to hold options
//builder.Services.Configure<ApiSettings>(
//    "api", apiSettings =>
//    {
//        apiSettings.Api = new Dictionary<string, string>();
//        Dictionary<string, string>? config = builder.Configuration.GetSection("Api").Get<Dictionary<string, string>>();
//        foreach (var key in config.Keys)
//            apiSettings.Api.Add(key, config[key]);
//    });

// A more correct way to map appSettings.json to Dictionary<string, string>
// We change the data class directly to be an instance of Dictionary<string, string>
builder.Services.Configure<ApiSettings>(
    "api",
    builder.Configuration.GetSection("Api")
    );


// ZX:      Uncomment if we want to use IOptions to read 'Application' configuration settings from `appsettings.json`
// Aside:   If we want a common data, we should just inject a singleton instance 🙄
//          That is assuming the data we want stays unchanged; 
//          If it changes periodically it might be better to use IOptionsMonitor
// builder.Services.Configure<ApplicationSettings>(
//     "application", 
//     builder.Configuration.GetSection("application")
//     );

builder.Services.Configure<JwtSettings>(
    "jwt",
    builder.Configuration.GetSection("Jwt")
    );


// ...End of adding your services here...


// Some ad-hoc development logging
if (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Application:Startup:DumpDebugInfo"))
{
    Console.WriteLine("# Development debug info:");
    Console.WriteLine("Jwt:ValidAudience    [{0}]", builder.Configuration["Jwt:ValidAudience"]);
    Console.WriteLine("Jwt:ValidIssuer      [{0}]", builder.Configuration["Jwt:ValidIssuer"]);
    Console.WriteLine("Jwt:SecretKey        [{0}]", builder.Configuration["Jwt:SecretKey"]);
}


var app = builder.Build();



// KIV: Don't see value for this vs custom HTTP logging
// See: https://github.com/serilog/serilog-aspnetcore
//app.UseSerilogRequestLogging(options =>
//{
//    // Customize the message template
//    options.MessageTemplate = "Handled {RequestPath}";
//    // Emit debug-level events instead of the defaults
//    options.GetLevel = (httpContext, elapsed, ex) => Serilog.Events.LogEventLevel.Debug;
//    // Attach additional properties to the request completion event
//    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
//    {
//        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
//        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
//    };
//});

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

// Map Endpoints

app.MapHealthChecks("/health").AllowAnonymous();

app.UseRequestHeaderLogging();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller=Home}/{action=Index}/{id?}");

// app.MapRazorPages();

// Debugging routes
if (builder.Configuration.GetValue<bool>("Application:Startup:DumpRoutes"))
{
    IActionDescriptorCollectionProvider actionProvider = app.Services.GetService<IActionDescriptorCollectionProvider>() ?? throw new Exception("No IActionDescriptorCollectionProvider");
    foreach (ActionDescriptor route in actionProvider.ActionDescriptors.Items)
        Console.WriteLine(route.DisplayName);
}


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

    //app.UseSerilogRequestLogging();


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
