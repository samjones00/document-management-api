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
        private readonly ILogger<AzureFunctions> _logger;

        public AzureFunctions(IMediator mediator, IValidator<UploadRequest> validator, ILogger<AzureFunctions> logger)
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
            try
            {
                var request = JsonConvert.DeserializeObject<UploadRequest>(await new StreamReader(httpRequest.Body)
                    .ReadToEndAsync());

                var filename = request.Filename;
                var byteArray = Convert.FromBase64String(request.Data);

                if (await UploadExists(filename))
                {
                    return new BadRequestObjectResult("File already exists.");
                }

                request.Bytes = byteArray;

                var validationResult = await _validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                    return new BadRequestObjectResult(validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));

                _logger.LogDebug($"Uploading {filename} to blob storage...");

                var uploadResponse = await _mediator.Send(new UploadBlobCommand(filename, byteArray));

                if (uploadResponse.IsSuccessful)
                {
                    return new CreatedResult("/", new
                    {
                        Filename = filename,
                        Size = byteArray.Length
                    });
                }

                return new BadRequestResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(message: "Error when uploading file.", exception: ex);

                return new InternalServerErrorResult();
            }
        }

        [FunctionName(nameof(CreateDocument))]
        public async Task CreateDocument([BlobTrigger("uploads/{filename}", Connection = "")]
            Stream stream, string filename)
        {
            try
            {
                var request = await _mediator.Send(new CreateDocumentCommand(filename, stream.Length));

                if(request.IsSuccessful)
                {
                    _logger.LogDebug($"Document created for {filename}");
                    return;
                }

                _logger.LogError($"Unable to create document for {filename}");

            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error when creating document for '{filename}.", exception: ex);
            }
        }

        [FunctionName(nameof(List))]
        public async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "list/{sortProperty?}/{sortDirection?}")] HttpRequest httpRequest, string sortProperty, string sortDirection = "Ascending")
        {
            try
            {
                var documents = await _mediator.Send(new GetDocumentCollectionQuery(sortProperty, sortDirection));

                if (!documents.IsSuccessful) return new InternalServerErrorResult();

                _logger.LogDebug($"{documents.Value.Count()} documents found.");

                return new OkObjectResult(documents.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: "Error when listing files.", exception: ex);

                return new InternalServerErrorResult();
            }
        }

        [FunctionName(nameof(Download))]
        public async Task<IActionResult> Download(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{filename}")]
            HttpRequest httpRequest,
            string filename)
        {
            try
            {
                var document = await _mediator.Send(new GetDocumentQuery(filename));

                if (!document.IsSuccessful) return new NotFoundObjectResult("File not found.");

                var blob = await _mediator.Send(new GetBlobAsMemoryStreamQuery(filename));

                if (!blob.IsSuccessful) return new NotFoundResult();

                return new FileContentResult(blob.Value.ToArray(), document.Value.ContentType)
                {
                    FileDownloadName = filename,
                    LastModified = document.Value.DateCreated
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(message: "Error when downloading file.", exception: ex);

                return new InternalServerErrorResult();
            }
        }

        [FunctionName(nameof(Delete))]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{filename}")] HttpRequest httpRequest,
            string filename)
        {
            try
            {
                if (!await UploadExists(filename)) return new NotFoundObjectResult("File not found.");

                _logger.LogDebug($"Deleting {filename} file...");

                var deleteBlobRequest = await _mediator.Send(new DeleteBlobCommand(filename));

                if (!deleteBlobRequest.IsSuccessful) return new BadRequestResult();

                _logger.LogDebug($"Deleting {filename} document...");

                var deleteDocumentRequest = await _mediator.Send(new DeleteDocumentCommand(filename));

                if (!deleteDocumentRequest.IsSuccessful) return new BadRequestResult();

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(message: $"Error when deleting '{filename}'.", exception: ex);

                return new InternalServerErrorResult();
            }
        }

        private async Task<bool> UploadExists(string filename)
        {
            var response = await _mediator.Send(new GetDocumentQuery(filename));

            return response.IsSuccessful;
        }
    }
}