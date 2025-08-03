using DocuSign.CodeExamples.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string ClientId = _config["ClientId"];
            string AuthServer = _config["AuthServer"];
            string ImpersonatedUserID = _config["ImpersonatedUserID"];
            string blobConnectionString = _config["BlobConnectionString"];
            string blobContainerName = _config["BlobcontainerNameUpload"];
            var keyPath = Path.Combine(Environment.CurrentDirectory, "private.key");
            byte[] privatekeyinByte = DsHelper.ReadFileContent(keyPath);



            CustomDocuSign customDocuSign = new CustomDocuSign();
            customDocuSign.InvokeDocusign("gupta.saket@outlook.com", "SaketG", "saket.guptaji@gmail.com","Gupta",ClientId,ImpersonatedUserID,AuthServer, privatekeyinByte, blobConnectionString, blobContainerName);


            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
