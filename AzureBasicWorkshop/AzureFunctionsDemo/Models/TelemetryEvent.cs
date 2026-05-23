using System.Text.Json.Serialization;

namespace AzureFunctionsDemo.Models;

/// <summary>
/// Событие телеметрии от IoT-устройства.
/// Используется в демо EventHubTrigger.
/// </summary>
public sealed class TelemetryEvent
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>Тип: "temperature", "humidity", "pressure".</summary>
    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = "measurement";

    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, string> Properties { get; set; } = [];
}
