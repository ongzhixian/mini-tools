using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniTools.HostApp.Services;

public class ExampleBearerService : BackgroundService
{
    private readonly ILogger<ExampleBearerService> logger;
    private readonly UserAuthenticationApiService userAuthenticationApiService;

    public ExampleBearerService(ILogger<ExampleBearerService> logger, UserAuthenticationApiService userAuthenticationApiService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAuthenticationApiService = userAuthenticationApiService ?? throw new ArgumentNullException(nameof(userAuthenticationApiService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // if not have bearer credentials, get it!

            await userAuthenticationApiService.GetBearerCredentialsAsync();

            await Task.Delay(5000);
        }
    }
}