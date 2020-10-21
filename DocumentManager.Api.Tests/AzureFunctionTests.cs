using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Models;
using DocumentManager.Core.Tests.Helpers;
using DocumentManager.Core.Validators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Api.Tests
{
    public class AzureFunctionTests
    {
        public readonly int MaximumFileSizeInBytes = 5242880;
        
        [Fact]
        public async Task Upload_GivenValidParameters_ShouldUploadToBlobStorage()
        {
            var filename = "example.pdf";
            var stream = FileHelper.GetExampleFile(filename);
            var base64 = FileHelper.ConvertToBase64(stream);

            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);
            var logger = new Mock<ILogger<AzureFunctions>>();
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(m => m.Send(It.IsAny<UploadBlobCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWrapper<bool>(true))
                .Verifiable($"{nameof(UploadBlobCommand)} was not called");

            var function = new AzureFunctions(mediator.Object, validator, logger.Object);

            Mock<HttpRequest> httpRequest = HttpRequestHelper.CreateMockRequest(new UploadRequest
            {
                Filename = filename,
                Data = base64
            });

            var expectedContentResult = new CreatedResult("/", new
            {
                Filename = filename,
                Size = stream.Length
            });

            var actionResult = await function.Upload(httpRequest.Object);
            var statusCodeResult = (IStatusCodeActionResult)actionResult;

            var createdResult = (ObjectResult) actionResult;

            Assert.Equal(createdResult.Value, expectedContentResult.Value);
            //createdResult.Value.shouldbe(expectedContentResult.Value);
            statusCodeResult.StatusCode.ShouldBe((int)HttpStatusCode.Created);
            //mediator.Verify(x => x.Send(It.IsAny<UploadBlobCommand>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
