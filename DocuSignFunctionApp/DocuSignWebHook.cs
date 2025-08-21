using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DocuSignFunctionApp;

public class DocuSignWebHook
{
    private readonly ILogger<DocuSignWebHook> _logger;

    public DocuSignWebHook(ILogger<DocuSignWebHook> logger)
    {
        _logger = logger;
    }

    [Function("DocuSignWebHook")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        _logger.LogInformation("Request method: {method}", req.Method);
        try
        {
            foreach (var header in req.Headers)
            {
                _logger.LogInformation("Header: {key} = {value}", header.Key, string.Join(", ", header.Value));
            }
            // Read and log body
            var requestBody = new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation("Request body: {body}", requestBody.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        // Log headers
       
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}