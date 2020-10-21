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
    public class GetBlobAsMemoryStreamTests
    {
        [Fact]
        public async Task Handler()
        {
            var filename = "example.pdf";

            var stream = StreamHelper.CreateExampleStream(10);

            var cancellationToken = new CancellationToken();
            var repository = new Mock<IStorageRepository>();
            repository.Setup(x => x.Get(filename, cancellationToken)).Returns(Task.FromResult(stream));

            var request = new GetBlobAsMemoryStream(filename);
            var handler = new GetBlobAsMemoryStreamHandler(repository.Object);

            var response = await handler.Handle(request, cancellationToken);

            response.IsSuccessful.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.ShouldBeOfType<MemoryStream>();
        }
    }
}
