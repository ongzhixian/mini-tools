using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiniTools.Web.Helpers.HttpMessageLogging;
using Serilog;

namespace MiniTools.Web.Services;

public static class AppSettingsKey
{
    public const string APPLICATION_ENABLE_HTTP_LOGGING = "Application:EnableHttpLogging";
}

public static class AppStartupService
{
    internal static void SetupLogging(ConfigureHostBuilder host)
    {
        host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
        });
    }

    internal static void AddAppSettings(ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        configuration.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);
    }

    internal static void SetupHttpLogging(ConfigurationManager configuration, IServiceCollection services)
    {
        if (configuration.GetValue<bool>(AppSettingsKey.APPLICATION_ENABLE_HTTP_LOGGING))
        {
            services.AddHttpLogging(logging =>
            {
                // Customize HTTP logging here.
                logging.LoggingFields = HttpLoggingFields.All;
                //logging.RequestHeaders.Add("My-Request-Header");
                //logging.ResponseHeaders.Add("My-Response-Header");
                //logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
            });
        }
    }

    internal static void SetupCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "DebugAllowAll",
                              builder =>
                                //   builder.WithOrigins("*")
                                builder.AllowAnyOrigin()
                                       .AllowAnyMethod()
                                       .AllowAnyHeader()

                              );
        });
    }

    internal static void SetupSwagger(ConfigurationManager configuration, IServiceCollection services)
    {
        
        OpenApiInfo openApiInfo = configuration.GetSection("Swagger:OpenApiInfo").Get<OpenApiInfo>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(openApiInfo.Version, openApiInfo);

            // Set the display order of the actions here.
            // options.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
            // options.OrderActionsBy((desc) => {});
        });



        //services.AddEndpointsApiExplorer();
        //services.AddSwaggerGen(options =>
        //{
        //    options.SwaggerDoc("v1", new OpenApiInfo
        //    {
        //        Version = "v1",
        //        Title = "Travel API",
        //        Description = "An ASP.NET Core Web API for managing Travel API items",
        //        TermsOfService = new Uri("https://example.com/terms"),
        //        Contact = new OpenApiContact
        //        {
        //            Name = "Example Contact",
        //            Url = new Uri("https://example.com/contact")
        //        },
        //        License = new OpenApiLicense
        //        {
        //            Name = "Example License",
        //            Url = new Uri("https://example.com/license")
        //        }
        //    });
        //});
    }

    internal static void SetupCookies(IServiceCollection services)
    {
        //var cookiePolicyOptions = new CookiePolicyOptions
        //{
        //    MinimumSameSitePolicy = SameSiteMode.Strict,
        //};

    }

    internal static void SetupAntiForgery(IServiceCollection services)
    {

        services.AddAntiforgery(opts => opts.Cookie.Name = "MyAntiforgeryCookie");

    }

    internal static void SetupSession(IServiceCollection services)
    {

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            //options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Name = "Cookie1";
        });

    }

    internal static void SetupAuthentication(ConfigurationManager configuration, IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            // options.Authority = "https://localhost:7241";
            // options.Audience = "weatherforecast";
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:ValidIssuer"],
                ValidAudience = configuration["Jwt:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]))
            };
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.Cookie.Name = "Cookie2";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(720);
                options.LoginPath = new PathString("/Login");
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;

            });
    }

    internal static void SetupAuthorization(IServiceCollection services)
    {
        //builder.Services.AddAuthorization();
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });




        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole",
                 policy => policy.RequireRole("Administrator"));
        });

        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("AtLeast21", policy =>
        //        policy.Requirements.Add(new MinimumAgeRequirement(21)));
        //});

    }

    internal static void SetupHttpClient(ConfigurationManager configuration, IServiceCollection services)
    {
        services.AddHttpClient(); // Add IHttpClientFactory

        services.AddHttpClient("authenticatedClient", (services, http) =>
        {
            IHttpContextAccessor httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
            //httpContextAccessor.HttpContext.Session

            if ((httpContextAccessor.HttpContext != null) && httpContextAccessor.HttpContext.Session.Keys.Contains("JWT"))
            {
                string token = httpContextAccessor.HttpContext.Session.GetString("JWT") ?? throw new NullReferenceException("Session[JWT] is null");
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

        });

        //builder.Services.AddHttpClient("authenticatedClient", (x) =>
        //{
        //    //System.Web.HttpContext.Current.User
        //    //Microsoft.AspNetCore.Identity.UserManager<>
        //    //Microsoft.AspNetCore.Http.HttpContext
        //    // IHttpContextAccessor _httpContextAccessor
        //    x.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "asddsad");
        //});


        // Place the following after all AddHttpClient registrations to implement our custom logging

        if (configuration.GetValue<bool>(AppSettingsKey.APPLICATION_ENABLE_HTTP_LOGGING))
        {
            //services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
            //services.AddSingleton<IHttpMessageHandlerBuilderFilter, CustomLoggingHttpMessageHandlerBuilderFilter>();
        }
    }

}