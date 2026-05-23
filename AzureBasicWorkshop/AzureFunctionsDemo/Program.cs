using AzureFunctionsDemo.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ═══════════════════════════════════════════════════════════════════════════
//  Azure Functions — .NET 8 Isolated Process
//
//  Isolated Process отличается от In-Process модели тем, что:
//    • Функция выполняется в ОТДЕЛЬНОМ процессе от хоста Functions
//    • Полная поддержка DI, Middleware и .NET 8 фич
//    • Program.cs + HostBuilder — как в обычном ASP.NET Core приложении
// ═══════════════════════════════════════════════════════════════════════════

var host = new HostBuilder()
    // Регистрирует стандартный pipeline: сериализация, логгирование, middleware
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // ─── Телеметрия → Azure Application Insights ─────────────────────────
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // ─── HTTP-клиент через фабрику (без утечки сокетов) ──────────────────
        services.AddHttpClient();

        // ─── Бизнес-сервисы (Scoped = новый экземпляр на каждый вызов функции)
        services.AddScoped<IOrderService, OrderService>();

        // ─── Кэширование ─────────────────────────────────────────────────────
        // Для работы с Redis раскомментируйте строки ниже и добавьте RedisConnection
        // в local.settings.json (и в Azure → Configuration → App Settings):
        //
        // services.AddStackExchangeRedisCache(opts =>
        //     opts.Configuration = context.Configuration["RedisConnection"]);
        //
        // Для локальной разработки — in-memory кэш (без внешних зависимостей):
        services.AddDistributedMemoryCache();
    })
    .Build();

await host.RunAsync();
