using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentManager.Common;
using DocumentManager.Common.Commands;
using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.WindowsAzure.Storage;

namespace DocumentManager.Api
{
    public class AzureFunctions
    {
        private readonly IUploadItemFactory _uploadItemFactory;
        private readonly CosmosClient _cosmosClient;
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly IMediator _mediator;

        public AzureFunctions(IUploadItemFactory uploadItemFactory, IConfiguration configuration)
        {
            _uploadItemFactory = uploadItemFactory;
            var cosmosConnectionString = configuration.GetConnectionString(Constants.Cosmos.ConnectionStringName);
            _cosmosClient = new CosmosClient(cosmosConnectionString);

            var storageConnectionString = configuration.GetConnectionString(Constants.Storage.ConnectionStringName);
            
            CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount);
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        [FunctionName(nameof(Upload))]
        public async Task<IActionResult> Upload(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest httpRequest,
            ILogger log)
        {
            var sw = new Stopwatch();
            sw.Start();

            var uploadRequest =
                JsonConvert.DeserializeObject<UploadRequest>(await new StreamReader(httpRequest.Body).ReadToEndAsync());
            var filename = uploadRequest.Filename;
            var byteArray = Convert.FromBase64String(uploadRequest.Data);

            log.LogInformation($"Uploading {filename}...");

            await _mediator.Send(new UploadDocumentCommand(filename, byteArray));

            return new CreatedResult("/", new
            {
                Filename = filename,
                Size = byteArray.Length
            });

            log.LogInformation($"Upload complete in {sw.ElapsedMilliseconds}ms - {filename}");
        }

        [FunctionName(nameof(CreateRecord))]
        public async Task CreateRecord([BlobTrigger("uploads/{name}", Connection = "")]
            Stream stream, string name,
            ILogger log)
        {
            var uploadItem = _uploadItemFactory.Create(name, stream.Length);

            var container = _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            await container.CreateItemAsync(uploadItem);

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {stream.Length} Bytes");
        }

        [FunctionName(nameof(List))]
        public async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]
            HttpRequest req,
            ILogger log)
        {
            var container = _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);

            var list = container.GetItemLinqQueryable<UploadItem>(true);

            if (list == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(list);
        }

        [FunctionName(nameof(Delete))]
        public async Task<IActionResult> Delete(

            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{filename}")] HttpRequest req,
            string filename,
            ILogger log)
        {
            var uri = UriFactory.CreateDocumentUri(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName,
                filename);

            log.LogInformation("C# HTTP trigger function processed a request.");

            var blobContainer = _cloudBlobClient.GetContainerReference(Constants.Storage.ContainerName);

            var blob = blobContainer.GetBlockBlobReference(filename);

            await blob.DeleteAsync();

            var container = _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            QueryDefinition queryDefinition = new QueryDefinition("select * from c");
            var queryResultSetIterator = container.GetItemQueryIterator<UploadItem>(queryDefinition);

            Microsoft.Azure.Cosmos.FeedResponse<UploadItem> currentResultSet =
                await queryResultSetIterator.ReadNextAsync();

            var doc = currentResultSet.FirstOrDefault(x => x.Filename == filename);

            await container.DeleteItemAsync<UploadItem>(doc.id, new PartitionKey(doc.ContentType));

            return new OkResult();
        }
    }
}
