using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Models;
using DocumentManager.Core.Tests;
using DocumentManager.Core.Tests.Helpers;
using DocumentManager.Core.Validators;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace DocumentManager.Api.Tests.Functions
{
    public class CreateDocumentTests
    {
        [Fact]
        public async Task CreateDocument_ShouldCreateDocument()
        {
            var filename = "example.pdf";
            var stream = StreamHelper.GetExampleFile(filename);

            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, Constants.MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");
            var validator = new UploadRequestValidator(configuration.Object);
            var logger = new FakeLogger<AzureFunctions>();

            var mediator = new Mock<IMediator>();
            mediator
                .Setup(m => m.Send(It.IsAny<CreateDocumentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWrapper<bool>(true));

            var function = new AzureFunctions(mediator.Object, validator, logger);

            await function.CreateDocument(stream, filename);

            var msg = logger.Logs[0];
            Assert.Contains("Document created", msg);
        }
    }
}
