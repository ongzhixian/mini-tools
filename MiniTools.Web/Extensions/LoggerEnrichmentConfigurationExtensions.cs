using MiniTools.Web.Helpers;
using Serilog;
using Serilog.Configuration;

namespace MiniTools.Web.Extensions
{
    /// <summary>
    /// This class is needed in order to hookup Serilog custom enrichers via JSON configuration.
    /// If configuring using code, we do not need this.
    /// We add it here for completeness.
    /// </summary>
    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithActivity(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.With<ActivityEnricher>();
        }

        public static LoggerConfiguration WithEventSourceOrName(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.With<EventSourceOrNameEnricher>();
        }
    }
}
