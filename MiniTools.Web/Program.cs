using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.OpenApi.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);

// 
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Is Development");
    Console.WriteLine("Jwt:ValidAudience    [{0}]", builder.Configuration["Jwt:ValidAudience"]);
    Console.WriteLine("Jwt:ValidIssuer      [{0}]", builder.Configuration["Jwt:ValidIssuer"]);
    Console.WriteLine("Jwt:SecretKey        [{0}]", builder.Configuration["Jwt:SecretKey"]);
}

builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
});

OpenApiInfo openApiInfo = builder.Configuration.GetSection("Swagger:OpenApiInfo").Get<OpenApiInfo>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(openApiInfo.Version, openApiInfo);

    // Set the display order of the actions here.
    // options.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
    // options.OrderActionsBy((desc) => {});
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "DebugAllowAll",
                      builder =>
                        //   builder.WithOrigins("*")
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader()

                      );
});



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new ()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

var app = builder.Build();

ILogger log = app.Services.GetRequiredService<ILogger<Program>>();

log.LogInformation("Application start");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("DebugAllowAll");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


//Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider
Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider actionProvider = app.Services.GetService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>();
log.Log(LogLevel.Information, "Available routes:");
var routes = actionProvider.ActionDescriptors.Items.Where(x => x.AttributeRouteInfo != null);
foreach (var route in routes)
{
    log.Log(LogLevel.Information, $"{route.AttributeRouteInfo.Template}");
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
