using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Commands
{
    public class DeleteBlobCommandTests
    {
        [Fact]
        public async Task DeleteBlobCommandTests_GivenValidParameters_ShouldReturnTrue()
        {
            var filename = "example.pdf";

            var cancellationToken = new CancellationToken();

            var repository = new Mock<IStorageRepository>();
   
            var request = new DeleteBlobCommand(filename);
            var handler = new DeleteBlobCommandHandler(repository.Object);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeTrue();
        }
    }
}
