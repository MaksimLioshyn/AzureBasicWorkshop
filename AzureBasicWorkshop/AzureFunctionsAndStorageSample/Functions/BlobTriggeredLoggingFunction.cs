using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsAndStorageSample.Functions;

/// <summary>
/// Blob Trigger функция - автоматически запускается при загрузке файла в контейнер.
/// ДЕМОНСТРАЦИЯ: событийно-ориентированная обработка (Event-Driven Architecture).
/// </summary>
public sealed class BlobTriggeredLoggingFunction
{
    private readonly ILogger<BlobTriggeredLoggingFunction> _logger;

    public BlobTriggeredLoggingFunction(ILogger<BlobTriggeredLoggingFunction> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Эта функция автоматически вызывается каждый раз, когда новый файл загружается в контейнер "demo-blobs".
    /// ДЕМОНСТРАЦИЯ: BlobTrigger - реактивная обработка файлов.
    /// 
    /// Применение:
    /// - Обработка изображений после загрузки (resize, thumbnail generation)
    /// - Парсинг CSV/Excel файлов
    /// - Сканирование файлов на вирусы
    /// - Извлечение метаданных
    /// - Автоматическая архивация или конвертация форматов
    /// </summary>
    /// <param name="blobStream">Содержимое загруженного файла в виде потока</param>
    /// <param name="name">Имя загруженного файла (автоматически передается триггером)</param>
    [Function("ProcessUploadedBlob")]
    public void Run(
        [BlobTrigger(DemoStorageNames.BlobTriggerPath, Connection = "AzureWebJobsStorage")] Stream blobStream, 
        string name)
    {
        // В учебном примере мы просто логируем информацию о файле
        // В реальном приложении здесь может быть любая обработка: распознавание текста, изменение размера изображения и т.д.
        _logger.LogInformation(
            "🔔 BlobTrigger activated! File '{BlobName}' uploaded. Size: {BlobSize} bytes", 
            name, 
            blobStream.Length);

        // Дополнительная обработка может включать:
        // - Чтение содержимого: using var reader = new StreamReader(blobStream);
        // - Обработка изображений с помощью ImageSharp
        // - Парсинг документов
        // - Отправка уведомлений о загрузке файла
    }
}