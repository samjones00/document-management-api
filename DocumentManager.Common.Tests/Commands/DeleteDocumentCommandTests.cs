using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Commands
{
    public class DeleteDocumentCommandTests
    {
        [Fact]
        public async Task Handler_GivenValidParameters_ReturnsTrue()
        {
            var filename = "example.pdf";
            var cancellationToken = new CancellationToken();

            var repository = new Mock<IDocumentRepository>();
            repository.Setup(x => x.Delete(filename, cancellationToken)).ReturnsAsync(true);

            var request = new DeleteDocumentCommand(filename);
            var handler = new DeleteDocumentCommandHandler(repository.Object);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeTrue();
        }

        [Fact]
        public async Task Handler_GivenNonExistentFilenameParameters_ReturnsEmptyValue()
        {
            var filename = "doesntexist.pdf";
            var cancellationToken = new CancellationToken();

            var repository = new Mock<IDocumentRepository>();
            repository.Setup(x => x.Delete(filename, cancellationToken)).ReturnsAsync(false);

            var request = new DeleteDocumentCommand(filename);
            var handler = new DeleteDocumentCommandHandler(repository.Object);

            var result = await handler.Handle(request, cancellationToken);

            result.IsSuccessful.ShouldBeFalse();
        }
    }
}
