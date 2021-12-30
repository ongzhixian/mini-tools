using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace MiniTools.Web.Helpers
{
    /// <summary>
    /// Originally want to write an enricher class that takes in certain headers in requests and log them out
    /// This cannot work because we have no way to get HttpContext / Request
    /// So we ended update writing an Activity enricher instead.
    /// For the problem of logging certain headers, we use a middleware instead.
    /// </summary>
    public class ActivityEnricher : ILogEventEnricher
    {
        // Note: Annoyingly, we cannot use DI
        //       (because enrichers being added to the pipeline need to be parameterless ( new()! )

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (Activity.Current == null)
                return;

            // TODO: Find out if there is any other way to get logging configuration of ActivityTrackingOptions
            //       (So that we can fine-tune which Activity properties to log; going with what seems to elementary)

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ActivityId", Activity.Current.Id));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", Activity.Current.SpanId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", Activity.Current.TraceId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentId", Activity.Current.ParentId));

            // We do not have use-case for the following; omit for now
            // logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceStateString", Activity.Current.TraceStateString));
            // logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Tags", Activity.Current.Tags));
            // logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Baggage", Activity.Current.Baggage));
        }
    }
}
