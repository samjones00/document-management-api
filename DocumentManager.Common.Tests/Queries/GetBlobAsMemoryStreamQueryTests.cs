using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Queries;
using DocumentManager.Core.Tests.Helpers;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Queries
{
    public class GetBlobAsMemoryStreamQueryTests
    {
        [Fact]
        public async Task Handler_GivenValidFilename_ReturnsMemoryStream()
        {
            var filename = "example.pdf";

            var stream = StreamHelper.CreateExampleStream();

            var cancellationToken = new CancellationToken();
            var repository = new Mock<IStorageRepository>();
            repository.Setup(x => x.Get(filename, cancellationToken)).Returns(Task.FromResult(stream));

            var request = new GetBlobAsMemoryStreamQuery(filename);
            var handler = new GetBlobAsMemoryStreamQueryHandler(repository.Object);

            var response = await handler.Handle(request, cancellationToken);

            response.IsSuccessful.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.ShouldBeOfType<MemoryStream>();
        }

        [Fact]
        public async Task Handler_GivenNonExistentFilename_ReturnsEmptyValue()
        {
            var filename = "example.pdf";

            var stream = StreamHelper.CreateExampleStream();

            var cancellationToken = new CancellationToken();
            var repository = new Mock<IStorageRepository>();
            repository.Setup(x => x.Get(filename, cancellationToken)).Returns(Task.FromResult((MemoryStream)null));

            var request = new GetBlobAsMemoryStreamQuery(filename);
            var handler = new GetBlobAsMemoryStreamQueryHandler(repository.Object);

            var response = await handler.Handle(request, cancellationToken);

            response.IsSuccessful.ShouldBeFalse();
            response.Value.ShouldBeNull();
        }
    }
}
