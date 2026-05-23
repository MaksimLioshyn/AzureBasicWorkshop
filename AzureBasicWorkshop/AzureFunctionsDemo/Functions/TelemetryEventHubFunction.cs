using System.Text.Json;
using AzureFunctionsDemo.Models;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Functions;

// ══════════════════════════════════════════════════════════════════════════
//  EVENT HUBS TRIGGER — потоковая обработка IoT-телеметрии
//
//  Ключевые отличия от очередей:
//    ✦ Пакетная обработка (batch) — до 100 событий за один вызов
//    ✦ Партиции — параллельная обработка разными экземплярами функции
//    ✦ Consumer Groups — несколько независимых потребителей одного потока
//    ✦ Retain — события хранятся до 90 дней (не удаляются после чтения)
//    ✦ Checkpoint — позиция чтения сохраняется в Storage для recover
//    ✦ Kafka-совместимый API
//
//  Сценарий: устройства шлют телеметрию → Event Hub →
//            эта функция агрегирует данные и выявляет аномалии
// ══════════════════════════════════════════════════════════════════════════
public sealed class TelemetryEventHubFunction
{
    private readonly ILogger<TelemetryEventHubFunction> _logger;

    public TelemetryEventHubFunction(ILogger<TelemetryEventHubFunction> logger)
        => _logger = logger;

    [Function(nameof(ProcessTelemetryBatch))]
    public Task ProcessTelemetryBatch(
        // IsBatched = true — получаем массив событий (до maxBatchSize из host.json)
        // ConsumerGroup — изолированный потребитель; другие Consumer Groups
        // читают тот же поток независимо
        [EventHubTrigger(
            "telemetry",
            Connection   = "EventHubConnection",
            ConsumerGroup = "$Default",
            IsBatched    = true)]
        EventData[] events,
        FunctionContext context)
    {
        _logger.LogInformation(
            "Event Hubs batch received: {Count} events", events.Length);

        // ─── Агрегация по устройствам ─────────────────────────────────────────
        // key = deviceId, value = (количество событий, сумма значений)
        var stats = new Dictionary<string, (int EventCount, double Sum, string Unit)>();

        foreach (var evt in events)
        {
            // EventBody — тело события как BinaryData
            // Метаданные: evt.Offset, evt.SequenceNumber, evt.EnqueuedTime, evt.PartitionKey
            TelemetryEvent? telemetry;
            try
            {
                telemetry = JsonSerializer.Deserialize<TelemetryEvent>(
                    evt.EventBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                // Пропускаем не парсируемые события с логированием
                _logger.LogWarning(ex,
                    "Cannot parse event at Offset={Offset}, SequenceNumber={Seq} — skipping",
                    evt.Offset, evt.SequenceNumber);
                continue;
            }

            if (telemetry is null) continue;

            // ─── Агрегация ────────────────────────────────────────────────────
            var key = telemetry.DeviceId;
            if (!stats.TryGetValue(key, out var existing))
                existing = (EventCount: 0, Sum: 0.0, Unit: telemetry.Unit);
            stats[key] = (EventCount: existing.EventCount + 1, Sum: existing.Sum + telemetry.Value, Unit: telemetry.Unit);

            // ─── Обнаружение аномалий ──────────────────────────────────────────
            // В продакшне: отправить уведомление в Service Bus или SignalR
            if (telemetry.EventType == "temperature" && telemetry.Value > 80.0)
            {
                _logger.LogWarning(
                    "ALERT: Device {DeviceId} temperature={Value}{Unit} at {Time} (Location: {Location})",
                    telemetry.DeviceId, telemetry.Value, telemetry.Unit,
                    telemetry.Timestamp, telemetry.Location ?? "unknown");
            }

            if (telemetry.EventType == "pressure" && telemetry.Value < 0.5)
            {
                _logger.LogWarning(
                    "ALERT: Device {DeviceId} low pressure={Value}{Unit} — possible sensor failure",
                    telemetry.DeviceId, telemetry.Value, telemetry.Unit);
            }
        }

        // ─── Сводная статистика по пакету ────────────────────────────────────
        foreach (var (deviceId, (evtCount, sum, unit)) in stats)
        {
            _logger.LogInformation(
                "Device {DeviceId}: {Count} readings, avg={Avg:F2} {Unit}",
                deviceId, evtCount, evtCount > 0 ? sum / evtCount : 0, unit);
        }

        // Event Hubs checkpoint сохраняется автоматически после успешного завершения функции
        return Task.CompletedTask;
    }
}
