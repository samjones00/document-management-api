using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Factories;
using Moq;
using Xunit;
using Shouldly;
using DocumentManager.Core.Interfaces;
using System;

namespace DocumentManager.Core.Tests.Queries
{
    public class CreateDocumentCommandTests
    {
        [Fact]
        public async Task Handler_GivenValidParameters_ReturnsTrue()
        {
            var filename = "example.pdf";
            var bytes = new byte[1000];
            var cancellationToken = new CancellationToken();

            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(dateCreated);

            var repository = new Mock<IDocumentRepository>();
            var factory = new DocumentFactory(dateTimeProvider.Object);

            var request = new CreateDocumentCommand(filename, bytes.Length);
            var handler = new CreateDocumentCommandHandler(repository.Object, factory);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeTrue();
        }

        [Fact]
        public async Task Handler_GivenEmptyFilenameParameters_ReturnsTrue()
        {
            var filename = "";
            var bytes = new byte[1000];
            var cancellationToken = new CancellationToken();

            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(dateCreated);

            var repository = new Mock<IDocumentRepository>();
            var factory = new DocumentFactory(dateTimeProvider.Object);

            var request = new CreateDocumentCommand(filename, bytes.Length);
            var handler = new CreateDocumentCommandHandler(repository.Object, factory);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeFalse();
        }

        [Fact]
        public async Task Handler_GivenEmptyFileParameters_ReturnsTrue()
        {
            var filename = "example.pdf";
            var bytes = new byte[0];
            var cancellationToken = new CancellationToken();

            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(dateCreated);

            var repository = new Mock<IDocumentRepository>();
            var factory = new DocumentFactory(dateTimeProvider.Object);

            var request = new CreateDocumentCommand(filename, bytes.Length);
            var handler = new CreateDocumentCommandHandler(repository.Object, factory);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeFalse();
        }
    }
}
