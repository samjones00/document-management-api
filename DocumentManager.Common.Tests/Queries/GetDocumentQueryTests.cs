using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Factories;
using Moq;
using Xunit;
using Shouldly;
using DocumentManager.Core.Interfaces;
using System;
using DocumentManager.Core.Models;
using DocumentManager.Core.Queries;

namespace DocumentManager.Core.Tests.Queries
{
    public class GetDocumentQueryTests
    {
        [Fact]
        public async Task Handler_GivenValidFilename_ReturnsDocument()
        {
            var filename = "example.pdf";
            var contentType = "application/pdf";
            var bytes = 1000;
            var cancellationToken = new CancellationToken();

            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);
            var repository = new Mock<IDocumentRepository>();
            repository.Setup(x => x.GetSingle(filename)).Returns(new Document(filename, bytes, contentType, dateCreated));

            var request = new GetDocumentQuery(filename);
            var handler = new GetDocumentQueryHandler(repository.Object);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeTrue();
            result.Value.ContentType.ShouldBe(contentType);
            result.Value.Filename.ShouldBe(filename);
            result.Value.Bytes.ShouldBe(bytes);
            result.Value.DateCreated.ShouldBe(dateCreated);
        }

        [Fact]
        public async Task Handler_GivenEmptyFilenameParameters_ReturnsNoValue()
        {
            var filename = "example.pdf";
            var cancellationToken = new CancellationToken();

            var repository = new Mock<IDocumentRepository>();
            repository.Setup(x => x.GetSingle(filename)).Returns((Document)null);

            var request = new GetDocumentQuery(filename);
            var handler = new GetDocumentQueryHandler(repository.Object);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeFalse();
            result.Value.ShouldBeNull();
        }
    }
}
