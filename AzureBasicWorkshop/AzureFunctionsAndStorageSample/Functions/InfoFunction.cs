using System.Net;
using AzureFunctionsAndStorageSample.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctionsAndStorageSample.Functions;

public sealed class InfoFunction
{
    [Function("Info")]
    public Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "info")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            project = "Azure Functions and Storage Sample",
            endpoints = new[]
            {
                "POST /api/setup",
                "POST /api/blobs/upload",
                "GET /api/blobs/{fileName}",
                "POST /api/queues/send",
                "POST /api/tables/save",
                "GET /api/tables/{partitionKey}/{rowKey}"
            },
            storage = new
            {
                container = DemoStorageNames.BlobContainerName,
                queue = DemoStorageNames.QueueName,
                table = DemoStorageNames.TableName
            }
        };

        return HttpResponseFactory.JsonAsync(request, HttpStatusCode.OK, payload, cancellationToken);
    }
}