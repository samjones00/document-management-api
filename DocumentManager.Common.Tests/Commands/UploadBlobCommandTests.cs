using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Commands
{
    public class UploadBlobCommandTests
    {
        [Fact]
        public async Task UploadBlobCommandHandler_GivenValidParameters_ShouldReturnTrue()
        {
            var filename = "example.pdf";
            var bytes = new byte[1000];
            var cancellationToken = new CancellationToken();

            var repository = new Mock<IStorageRepository>();

            var request = new UploadBlobCommand(filename, bytes);
            var handler = new UploadBlobCommandHandler(repository.Object);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeTrue();
        }
    }
}
