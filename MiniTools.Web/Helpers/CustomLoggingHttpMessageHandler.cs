using System.Net;

namespace Dn6Poc.DocuMgmtPortal.Logging
{
    // From: https://github.com/dotnet/extensions/blob/release/3.1/src/HttpClientFactory/Http/src/Logging/LoggingHttpMessageHandler.cs
    public class CustomLoggingHttpMessageHandler : DelegatingHandler
    {
        private ILogger _logger;

        public CustomLoggingHttpMessageHandler(ILogger logger)
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

            // Not using a scope here because we always expect this to be at the end of the pipeline, thus there's
            // not really anything to surround.
            Log.RequestStart(_logger, request);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            Log.RequestEnd(_logger, response, stopwatch.GetElapsedTime());

            return response;
        }

        private static class Log
        {
            public static class EventIds
            {
                public static readonly EventId RequestStart = new EventId(100, "RequestStart");
                public static readonly EventId RequestEnd = new EventId(101, "RequestEnd");

                public static readonly EventId RequestHeader = new EventId(102, "RequestHeader");
                public static readonly EventId ResponseHeader = new EventId(103, "ResponseHeader");
            }

            private static readonly Action<ILogger, HttpMethod, Uri, Exception> _requestStart = LoggerMessage.Define<HttpMethod, Uri>(
                LogLevel.Information,
                EventIds.RequestStart,
                "Sending HTTP request {HttpMethod} {Uri}");

            private static readonly Action<ILogger, double, HttpStatusCode, Exception> _requestEnd = LoggerMessage.Define<double, HttpStatusCode>(
                LogLevel.Information,
                EventIds.RequestEnd,
                "Received HTTP response after {ElapsedMilliseconds}ms - {StatusCode}");

            public static void RequestStart(ILogger logger, HttpRequestMessage request)
            {
                _requestStart(logger, request.Method, request.RequestUri, null);

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.Log(
                        LogLevel.Trace,
                        EventIds.RequestHeader,
                        new CustomHttpHeadersLogValue(CustomHttpHeadersLogValue.Kind.Request, request.Headers, request.Content?.Headers),
                        null,
                        (state, ex) => state.ToString());
                }
            }

            public static void RequestEnd(ILogger logger, HttpResponseMessage response, TimeSpan duration)
            {
                _requestEnd(logger, duration.TotalMilliseconds, response.StatusCode, null);

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