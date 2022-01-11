using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiniTools.HostApp.Models
{
    public class AirTemperatureInfo
    {
        [JsonPropertyName("api_info")]
        public ApiInfo? ApiInfo { get; set; }

        [JsonPropertyName("metadata")]
        public MetaData? MetaData { get; set; }

        [JsonPropertyName("items")]
        public IEnumerable<TimestampedReadings>? Items { get; set; }
    }

    public class TimestampedReadings
    {

        [JsonPropertyName("timestamp")]
        public string TimeStamp { get; set; } = string.Empty;


        [JsonPropertyName("readings")]
        public IEnumerable<StationTemperatureReading>? Readings { get; set; }
    }

    public class StationTemperatureReading
    {
        [JsonPropertyName("station_id")]
        public string StationId { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }


    public class MetaData
    {
        [JsonPropertyName("stations")]
        public IEnumerable<StationMetaData> Stations { get; set; } = new List<StationMetaData>();

        [JsonPropertyName("reading_type")]
        public string ReadingType { get; set; } = string.Empty;

        [JsonPropertyName("reading_unit")]
        public string ReadingUnit { get; set; } = string.Empty;
    }

    public class StationMetaData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public Location? Location { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

    }

    public class ApiInfo
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    
}
