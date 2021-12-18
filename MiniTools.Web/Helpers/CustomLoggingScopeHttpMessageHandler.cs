using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace Dn6Poc.DocuMgmtPortal.Logging
{
    // From: https://github.com/dotnet/extensions/blob/release/3.1/src/HttpClientFactory/Http/src/Logging/LoggingScopeHttpMessageHandler.cs
    public class CustomLoggingScopeHttpMessageHandler : DelegatingHandler
    {
        private ILogger _logger;

        public CustomLoggingScopeHttpMessageHandler(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var stopwatch = CustomValueStopwatch.StartNew();

            using (Log.BeginRequestPipelineScope(_logger, request))
            {
                await Log.RequestPipelineStartAsync(_logger, request);
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                Log.RequestPipelineEnd(_logger, response, stopwatch.GetElapsedTime());

                return response;
            }
        }

        private static class Log
        {
            public static class EventIds
            {
                public static readonly EventId PipelineStart = new EventId(100, "RequestPipelineStart");
                public static readonly EventId PipelineEnd = new EventId(101, "RequestPipelineEnd");

                public static readonly EventId RequestHeader = new EventId(102, "RequestPipelineRequestHeader");
                public static readonly EventId ResponseHeader = new EventId(103, "RequestPipelineResponseHeader");
            }

            private static readonly Func<ILogger, HttpMethod, Uri, IDisposable> _beginRequestPipelineScope = LoggerMessage.DefineScope<HttpMethod, Uri>("HTTP {HttpMethod} {Uri}");

            private static readonly Action<ILogger, HttpMethod, Uri, Exception> _requestPipelineStart = LoggerMessage.Define<HttpMethod, Uri>(
                LogLevel.Information,
                EventIds.PipelineStart,
                "Start processing HTTP request {HttpMethod} {Uri}");

            private static readonly Action<ILogger, double, HttpStatusCode, Exception> _requestPipelineEnd = LoggerMessage.Define<double, HttpStatusCode>(
                LogLevel.Information,
                EventIds.PipelineEnd,
                "End processing HTTP request after {ElapsedMilliseconds}ms - {StatusCode}");

            public static IDisposable BeginRequestPipelineScope(ILogger logger, HttpRequestMessage request)
            {
                return _beginRequestPipelineScope(logger, request.Method, request.RequestUri);
            }

            public static async Task RequestPipelineStartAsync(ILogger logger, HttpRequestMessage request)
            {
                _requestPipelineStart(logger, request.Method, request.RequestUri, null);

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.Log(
                        LogLevel.Trace,
                        EventIds.RequestHeader,
                        new CustomHttpHeadersLogValue(CustomHttpHeadersLogValue.Kind.Request, request.Headers, request.Content?.Headers),
                        null,
                        (state, ex) => state.ToString());

                    if (request.Content != null)
                    {
                        string actualContent = await request.Content.ReadAsStringAsync();
                        logger.Log(LogLevel.Trace, "X-CONTENT:" + actualContent);
                    }

                }

                
            }

            public static void RequestPipelineEnd(ILogger logger, HttpResponseMessage response, TimeSpan duration)
            {
                _requestPipelineEnd(logger, duration.TotalMilliseconds, response.StatusCode, null);

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.Log(
                        LogLevel.Trace,
                        EventIds.ResponseHeader,
                        new CustomHttpHeadersLogValue(CustomHttpHeadersLogValue.Kind.Response, response.Headers, response.Content?.Headers),
                        null,
                        (state, ex) => state.ToString());
                }
            }
        }
    }
}
