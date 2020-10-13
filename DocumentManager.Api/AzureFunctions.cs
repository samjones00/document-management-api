using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DocumentManager.Common;
using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace DocumentManager.Api
{
    public class AzureFunctions
    {
        private readonly IUploadItemFactory _uploadItemFactory;
        public AzureFunctions(IUploadItemFactory uploadItemFactory)
        {
            _uploadItemFactory = uploadItemFactory;
        }

        [FunctionName(nameof(List))]
        public async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]
            HttpRequest req,
            [CosmosDB(
                databaseName: Constants.Cosmos.DatabaseName,
                collectionName:  Constants.Cosmos.ContainerName,
                ConnectionStringSetting = Constants.Cosmos.ConnectionStringName
                )
            ]
            IEnumerable<UploadItem> productItem,
            ILogger log)
        {
            if (productItem == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(productItem);
        }

        [FunctionName(nameof(Delete))]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{id}")] HttpRequest req,
            [Blob("uploads", Connection = "StorageConnectionString")] CloudBlobContainer outputContainer, string filename,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["id"];

            var cloudBlockBlob = outputContainer.GetBlockBlobReference(filename);


             await cloudBlockBlob.DeleteIfExistsAsync();

            return new OkResult();
        }

        [FunctionName(nameof(Upload))]
        public async Task<IActionResult> Upload(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest httpRequest,
            [Blob(Constants.Storage.ContainerName, Connection = Constants.Storage.ConnectionStringName)] CloudBlobContainer outputContainer,
            ILogger log)
        {
            log.LogInformation("Uploading file...");
            var sw = Stopwatch.StartNew();
            await outputContainer.CreateIfNotExistsAsync();

            var requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();

            var uploadRequest = JsonConvert.DeserializeObject<UploadRequest>(requestBody);
            var filename = uploadRequest.Filename;

            var cloudBlockBlob = outputContainer.GetBlockBlobReference(filename);
            var bytes = Convert.FromBase64String(uploadRequest.Data);

            await cloudBlockBlob.UploadFromStreamAsync(new MemoryStream(bytes));

            log.LogInformation($"Upload complete in {sw.ElapsedMilliseconds}ms - {filename}");
            return new CreatedResult("/", filename);
        }

        [FunctionName(nameof(CreateRecord))]
        public async Task CreateRecord([BlobTrigger("uploads/{name}", Connection = "")]
            Stream stream, string name,
            [CosmosDB(
                databaseName: Constants.Cosmos.DatabaseName,
                collectionName:  Constants.Cosmos.ContainerName,
                ConnectionStringSetting = Constants.Cosmos.ConnectionStringName)]
            IAsyncCollector<object> uploadItems,
            ILogger log)
        {
            var uploadItem = _uploadItemFactory.Create(name, stream.Length);

            await uploadItems.AddAsync(uploadItem);

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {stream.Length} Bytes");
        }
    }
}
