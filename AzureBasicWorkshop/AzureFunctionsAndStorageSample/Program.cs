using AzureFunctionsAndStorageSample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ===== ТОЧКА ВХОДА В AZURE FUNCTIONS =====
// Это стартовый файл приложения Azure Functions для Isolated Worker Process модели (.NET 8)

var host = new HostBuilder()
    // ConfigureFunctionsWorkerDefaults() настраивает все необходимое для работы Azure Functions:
    // - регистрирует сервисы для обработки HTTP-запросов, триггеров и биндингов
    // - настраивает логирование и конфигурацию
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Регистрируем наш сервис для работы с Azure Storage в DI-контейнере
        // Singleton означает, что один экземпляр будет использоваться всеми функциями
        services.AddSingleton<IStorageDemoService, StorageDemoService>();
    })
    .Build();

// Запускаем хост приложения - теперь Azure Functions готов принимать запросы
host.Run();