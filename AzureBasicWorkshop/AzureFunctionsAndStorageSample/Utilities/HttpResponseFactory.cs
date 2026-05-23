using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctionsAndStorageSample.Utilities;

public static class HttpResponseFactory
{
    public static async Task<HttpResponseData> JsonAsync<T>(
        HttpRequestData request,
        HttpStatusCode statusCode,
        T payload,
        CancellationToken cancellationToken = default)
    {
        var response = request.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(payload, cancellationToken);
        return response;
    }

    public static async Task<HttpResponseData> TextAsync(
        HttpRequestData request,
        HttpStatusCode statusCode,
        string text,
        CancellationToken cancellationToken = default)
    {
        var response = request.CreateResponse(statusCode);
        await response.WriteStringAsync(text, cancellationToken);
        return response;
    }
}