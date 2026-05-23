using AzureFunctionsDemo.Models;
using AzureFunctionsDemo.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsDemo.Functions;

// ══════════════════════════════════════════════════════════════════════════
//  TIMER TRIGGER — ежедневный отчёт по заказам
//
//  CRON-выражение: "секунды минуты часы день месяц деньнедели"
//
//  Примеры CRON:
//    "0 0 8 * * *"      → каждый день в 08:00 UTC
//    "0 */5 * * * *"    → каждые 5 минут
//    "0 0 0 * * 1"      → каждый понедельник в полночь
//    "0 0 12 1 * *"     → 1-го числа каждого месяца в 12:00
//
//  IsPastDue — true если хост был остановлен во время планового запуска.
//  RunOnStartup — запускать сразу при старте (удобно для отладки).
// ══════════════════════════════════════════════════════════════════════════
public sealed class DailyReportFunction
{
    private readonly IOrderService _orderService;
    private readonly ILogger<DailyReportFunction> _logger;

    public DailyReportFunction(IOrderService orderService, ILogger<DailyReportFunction> logger)
    {
        _orderService = orderService;
        _logger       = logger;
    }

    // ─── Каждый день в 08:00 UTC ──────────────────────────────────────────────
    [Function(nameof(GenerateDailyReport))]
    public async Task GenerateDailyReport(
#if DEBUG
        // В DEBUG: запускается сразу при старте приложения (для удобства тестирования)
        [TimerTrigger("0 0 8 * * *", RunOnStartup = true)]
#else
        [TimerTrigger("0 0 8 * * *")]
#endif
        TimerInfo timerInfo,
        FunctionContext context)
    {
        // ─── Пропущенный плановый запуск ─────────────────────────────────────
        // Если хост Functions был недоступен в момент планового запуска,
        // таймер запустится при следующем старте хоста с IsPastDue = true
        if (timerInfo.IsPastDue)
        {
            _logger.LogWarning(
                "Timer ran late! Last scheduled run was: {LastRun}",
                timerInfo.ScheduleStatus?.Last.ToString("u") ?? "unknown");
        }

        var reportTime = DateTimeOffset.UtcNow;
        _logger.LogInformation(
            "Daily report generation started at {ReportTime}", reportTime);

        // ─── Сбор данных ──────────────────────────────────────────────────────
        var orders = await _orderService.GetRecentAsync(top: 1000);

        if (orders.Count == 0)
        {
            _logger.LogInformation("No orders found — report skipped");
            return;
        }

        // ─── Агрегация ────────────────────────────────────────────────────────
        var today       = DateTimeOffset.UtcNow.Date;
        var todayOrders = orders.Where(o => o.CreatedAt.Date == today).ToList();

        var byStatus    = orders.GroupBy(o => o.Status)
                                .ToDictionary(g => g.Key, g => g.Count());

        var totalRevenue = orders
            .Where(o => o.Status == OrderStatus.Completed)
            .Sum(o => o.TotalAmount);

        var avgOrderValue = orders.Count > 0
            ? orders.Average(o => (double)o.TotalAmount)
            : 0;

        var topCustomers = orders
            .GroupBy(o => o.CustomerId)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new { CustomerId = g.Key, OrderCount = g.Count() })
            .ToList();

        // ─── Structured Logging → Application Insights ────────────────────────
        // Эти логи попадают в Application Insights и доступны через KQL-запросы
        _logger.LogInformation(
            "Daily Report | Date={Date} | Total={Total} | TodayOrders={Today} | " +
            "Revenue={Revenue:C} | AvgOrder={Avg:C} | Pending={Pending} | " +
            "Processing={Processing} | Completed={Completed} | Cancelled={Cancelled} | Failed={Failed}",
            today.ToString("yyyy-MM-dd"),
            orders.Count,
            todayOrders.Count,
            totalRevenue,
            avgOrderValue,
            byStatus.GetValueOrDefault(OrderStatus.Pending),
            byStatus.GetValueOrDefault(OrderStatus.Processing),
            byStatus.GetValueOrDefault(OrderStatus.Completed),
            byStatus.GetValueOrDefault(OrderStatus.Cancelled),
            byStatus.GetValueOrDefault(OrderStatus.Failed));

        foreach (var customer in topCustomers)
        {
            _logger.LogInformation(
                "Top customer: {CustomerId} with {Count} orders",
                customer.CustomerId, customer.OrderCount);
        }

        // В продакшне здесь можно:
        // - Записать отчёт в Azure Blob Storage (CSV/PDF)
        // - Отправить email через SendGrid binding
        // - Записать агрегаты в Azure SQL DB / Cosmos DB
        // - Опубликовать событие в Service Bus

        _logger.LogInformation("Daily report completed in {Elapsed}ms",
            (DateTimeOffset.UtcNow - reportTime).TotalMilliseconds);
    }
}
