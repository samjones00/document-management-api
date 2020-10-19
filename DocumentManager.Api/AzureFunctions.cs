using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Models;
using DocumentManager.Core.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DocumentManager.Api
{
    public class AzureFunctions
    {
        private readonly IMediator _mediator;
        private readonly IValidator<UploadRequest> _validator;
        private readonly ILogger _logger;

        public AzureFunctions(IMediator mediator, IValidator<UploadRequest> validator, ILogger logger)
        {
            _mediator = mediator;
            _validator = validator;
            _logger = logger;
        }

        [FunctionName(nameof(Upload))]
        public async Task<IActionResult> Upload(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest httpRequest)
        {
            var uploadRequest = JsonConvert.DeserializeObject<UploadRequest>(await new StreamReader(httpRequest.Body)
                .ReadToEndAsync());

            var filename = uploadRequest.Filename;
            var byteArray = Convert.FromBase64String(uploadRequest.Data);

            if (await UploadExists(filename))
            {
                return new BadRequestObjectResult("File already exists.");
            }

            uploadRequest.Bytes = byteArray;

            var validationResult = _validator.Validate(uploadRequest);

            if (!validationResult.IsValid)
                return new BadRequestObjectResult(validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));

            _logger.LogInformation($"Uploading {filename} to blob storage...");

            await _mediator.Send(new UploadBlobCommand(filename, byteArray));

            _logger.LogInformation($"Upload of {filename} completed.");

            return new CreatedResult("/", new
            {
                Filename = filename,
                Size = byteArray.Length
            });
        }

        [FunctionName(nameof(CreateDocument))]
        public async Task CreateDocument([BlobTrigger("uploads/{filename}", Connection = "")]
            Stream stream, string filename)
        {
            await _mediator.Send(new CreateDocumentCommand(filename, stream.Length));

            _logger.LogInformation($"Document created for {filename}");
        }

        [FunctionName(nameof(List))]
        public async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "list")] HttpRequest req)
        {
            var property = req.Query["property"];
            var order = req.Query["order"];

            var documents = await _mediator.Send(new GetDocumentsQuery(property, ListSortDirection.Ascending));

            if (!documents.IsSuccessful) return new NotFoundResult();

            _logger.LogInformation($"{documents.Value.Count} documents found");

            return new OkObjectResult(documents);
        }

        [FunctionName(nameof(Download))]
        public async Task<IActionResult> Download(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{filename}")]
            HttpRequest req,
            string filename)
        {
            var document = await _mediator.Send(new GetDocumentsQuery(x =>
                x.Filename.Equals(filename, StringComparison.InvariantCultureIgnoreCase)));

            if (!document.IsSuccessful) return new NotFoundObjectResult("File not found.");

            var blob = await _mediator.Send(new GetBlobAsByteArrayQuery(filename));

            if (!blob.IsSuccessful) return new InternalServerErrorResult();

            return new FileContentResult(blob.Value, document.Value.First().ContentType)
            {
                FileDownloadName = filename
            };
        }

        [FunctionName(nameof(Delete))]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{filename}")] HttpRequest req,
            string filename)
        {
            if (!await UploadExists(filename)) return new NotFoundObjectResult("File not found.");

            _logger.LogInformation($"Deleting {filename} file...");
            if (!await _mediator.Send(new DeleteBlobCommand(filename))) return new BadRequestResult();

            _logger.LogInformation($"Deleting {filename} document...");
            if (!await _mediator.Send(new DeleteDocumentCommand(filename))) return new BadRequestResult();

            return new OkResult();
        }

        private async Task<bool> UploadExists(string filename)
        {
            var response = await _mediator.Send(new GetDocumentsQuery(x =>
                x.Filename.Equals(filename, StringComparison.InvariantCultureIgnoreCase)));

            if (!response.IsSuccessful)
                return false;

            return response.Value.Any();
        }
    }
}