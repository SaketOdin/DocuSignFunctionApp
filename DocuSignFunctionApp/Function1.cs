using DocuSign.CodeExamples.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace DocuSignFunctionApp
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly IConfiguration _config;

        public Function1(ILogger<Function1> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                string ClientId = _config["ClientId"];
                string AuthServer = _config["AuthServer"];
                string ImpersonatedUserID = _config["ImpersonatedUserID"];
                string blobConnectionString = _config["BlobConnectionString"];
                string blobContainerName = _config["BlobcontainerNameUpload"];
                var keyPath = Path.Combine(Environment.CurrentDirectory, "private.key");
                byte[] privatekeyinByte = DsHelper.ReadFileContent(keyPath);

                _logger.LogInformation("ClientId: {ClientId}, AuthServer: {AuthServer}, ImpersonatedUserID: {ImpersonatedUserID}, BlobConnectionString: {BlobConnectionString}, BlobContainerName: {BlobContainerName}",
                    ClientId, AuthServer, ImpersonatedUserID, blobConnectionString, blobContainerName);
                string requestBody = await  new  StreamReader(req.Body).ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return new BadRequestObjectResult("Request body is empty.");
                }
                XDocument xmlDoc = XDocument.Parse(requestBody);
                _logger.LogInformation($"Request body: {requestBody}");

                CustomDocuSign customDocuSign = new CustomDocuSign();
                customDocuSign.InvokeDocusign(xmlDoc ,"gupta.saket@outlook.com", "SaketG", "saket.guptaji@gmail.com", "Gupta", ClientId, ImpersonatedUserID, AuthServer, privatekeyinByte, blobConnectionString, blobContainerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace + ex, "An error occurred while processing the request.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }


            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
