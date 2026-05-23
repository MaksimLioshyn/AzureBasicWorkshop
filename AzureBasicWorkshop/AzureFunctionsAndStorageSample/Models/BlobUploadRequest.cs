namespace AzureFunctionsAndStorageSample.Models;

/// <summary>
/// Модель запроса для загрузки файла в Blob Storage.
/// Используется при HTTP POST запросах к /api/blobs/upload
/// </summary>
public sealed class BlobUploadRequest
{
    /// <summary>
    /// Имя файла, который будет создан в Blob Storage.
    /// Пример: "document.txt", "image.jpg"
    /// </summary>
    public string? FileName { get; init; }

    /// <summary>
    /// Текстовое содержимое файла.
    /// Для демо-проекта используем текст, в реальных проектах может быть Base64 для бинарных данных.
    /// </summary>
    public string? Content { get; init; }
}