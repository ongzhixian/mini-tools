using Microsoft.Extensions.Http;
using System.Net;

namespace Dn6Poc.DocuMgmtPortal.Logging
{
    // Base on: https://docs.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/5.0/http-httpclient-instances-log-integer-status-codes
    // From: https://github.com/dotnet/extensions/blob/release/3.1/src/HttpClientFactory/Http/src/Logging/LoggingHttpMessageHandlerBuilderFilter.cs
    internal class CustomLoggingHttpMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly ILoggerFactory _loggerFactory;

        public CustomLoggingHttpMessageHandlerBuilderFilter(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _loggerFactory = loggerFactory;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            return (builder) =>
            {
                // Run other configuration first, we want to decorate.
                next(builder);

                var loggerName = !string.IsNullOrEmpty(builder.Name) ? builder.Name : "Default";

                // We want all of our logging message to show up as-if they are coming from HttpClient,
                // but also to include the name of the client for more fine-grained control.
                var outerLogger = _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.LogicalHandler");
                var innerLogger = _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.ClientHandler");

                // The 'scope' handler goes first so it can surround everything.
                builder.AdditionalHandlers.Insert(0, new CustomLoggingScopeHttpMessageHandler(outerLogger));

                // We want this handler to be last so we can log details about the request after
                // service discovery and security happen.
                builder.AdditionalHandlers.Add(new CustomLoggingHttpMessageHandler(innerLogger));

            };
        }
    }

    // The below is based https://www.stevejgordon.co.uk/httpclientfactory-asp-net-core-logging

    //public class HttpClientLoggingFilter : IHttpMessageHandlerBuilderFilter
    //{
    //    private readonly ILoggerFactory _loggerFactory;

    //    public HttpClientLoggingFilter(ILoggerFactory loggerFactory)
    //    {
    //        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    //    }

    //    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    //    {
    //        if (next == null)
    //        {
    //            throw new ArgumentNullException(nameof(next));
    //        }

    //        return (builder) =>
    //        {
    //            // Run other configuration first, we want to decorate.
    //            next(builder);

    //            var outerLogger =
    //                _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{builder.Name}.LogicalHandler");

    //            builder.AdditionalHandlers.Insert(0, new HttpClientLoggingScopeHttpMessageHandler(outerLogger));
    //        };
    //    }
    //}


    //public class HttpClientLoggingScopeHttpMessageHandler : DelegatingHandler
    //{
    //    private readonly ILogger _logger;

    //    public HttpClientLoggingScopeHttpMessageHandler(ILogger logger)
    //    {
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }

    //    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
    //        CancellationToken cancellationToken)
    //    {
    //        if (request == null)
    //        {
    //            throw new ArgumentNullException(nameof(request));
    //        }

    //        using (Log.BeginRequestPipelineScope(_logger, request))
    //        {
    //            Log.RequestPipelineStart(_logger, request);
    //            var response = await base.SendAsync(request, cancellationToken);
    //            Log.RequestPipelineEnd(_logger, response);

    //            return response;
    //        }
    //    }

    //    private static class Log
    //    {
    //        private static class EventIds
    //        {
    //            public static readonly EventId PipelineStart = new EventId(100, "RequestPipelineStart");
    //            public static readonly EventId PipelineEnd = new EventId(101, "RequestPipelineEnd");
    //        }

    //        private static readonly Func<ILogger, HttpMethod, Uri, string, IDisposable> _beginRequestPipelineScope =
    //            LoggerMessage.DefineScope<HttpMethod, Uri, string>(
    //                "HTTP {HttpMethod} {Uri} {CorrelationId}");

    //        private static readonly Action<ILogger, HttpMethod, Uri, string, Exception> _requestPipelineStart =
    //            LoggerMessage.Define<HttpMethod, Uri, string>(
    //                LogLevel.Information,
    //                EventIds.PipelineStart,
    //                "Start processing HTTP request {HttpMethod} {Uri} [Correlation: {CorrelationId}]");

    //        private static readonly Action<ILogger, HttpStatusCode, Exception> _requestPipelineEnd =
    //            LoggerMessage.Define<HttpStatusCode>(
    //                LogLevel.Information,
    //                EventIds.PipelineEnd,
    //                "End processing HTTP request - {StatusCode}");

    //        public static IDisposable BeginRequestPipelineScope(ILogger logger, HttpRequestMessage request)
    //        {
    //            var correlationId = GetCorrelationIdFromRequest(request);
    //            return _beginRequestPipelineScope(logger, request.Method, request.RequestUri, correlationId);
    //        }

    //        public static void RequestPipelineStart(ILogger logger, HttpRequestMessage request)
    //        {
    //            var correlationId = GetCorrelationIdFromRequest(request);
    //            _requestPipelineStart(logger, request.Method, request.RequestUri, correlationId, null);
    //        }

    //        public static void RequestPipelineEnd(ILogger logger, HttpResponseMessage response)
    //        {
    //            _requestPipelineEnd(logger, response.StatusCode, null);
    //        }

    //        private static string GetCorrelationIdFromRequest(HttpRequestMessage request)
    //        {
    //            var correlationId = "Not set";

    //            if (request.Headers.TryGetValues("X-Correlation-ID", out var values))
    //            {
    //                correlationId = values.First();
    //            }

    //            return correlationId;
    //        }
    //    }
    //}
}
