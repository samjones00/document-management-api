using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using DocumentManager.Core.Queries;
using DocumentManager.Core.Tests;
using DocumentManager.Core.Tests.Helpers;
using DocumentManager.Core.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Api.Tests.Functions
{
    public class DownloadTests
    {
        [Fact]
        public async Task Download_GivenValidParameters_ShouldReturnFileContentResult()
        {
            var filename = "example.pdf";
            var contentType = "application/pdf";
            var bytes = 1000;

            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, Constants.MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);
            var logger = new Mock<ILogger<AzureFunctions>>();
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(m => m.Send(It.IsAny<GetDocumentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWrapper<Document>(
                    new Document(filename, bytes, "application/pdf", new DateTime(02, 03, 04, 05, 06, 07)), true));

            mediator
                .Setup(m => m.Send(It.IsAny<GetBlobAsMemoryStreamQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWrapper<MemoryStream>(StreamHelper.CreateExampleStream(bytes), true));

            var function = new AzureFunctions(mediator.Object, validator, logger.Object);

            var actionResult = await function.Download(new Mock<HttpRequest>().Object, filename);
            actionResult.ShouldBeOfType<FileContentResult>();

            var createdResult = (FileResult) actionResult;
            createdResult.FileDownloadName.ShouldBe(filename);
            createdResult.ContentType.ShouldBe(contentType);
        }

        [Fact]
        public async Task Download_GivenNonExistentFilename_ShouldReturnNotFoundObjectResult()
        {
            var filename = "example.pdf";

            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, Constants.MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);
            var logger = new Mock<ILogger<AzureFunctions>>();
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(m => m.Send(It.IsAny<GetDocumentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWrapper<Document>(false));

            var function = new AzureFunctions(mediator.Object, validator, logger.Object);

            var actionResult = await function.Download(new Mock<HttpRequest>().Object, filename);
            actionResult.ShouldBeOfType<NotFoundObjectResult>();
        }
    }
}
