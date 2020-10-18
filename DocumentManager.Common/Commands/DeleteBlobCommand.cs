using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentManager.Core.Commands
{
    public class DeleteBlobCommand : IRequest<bool>
    {
        public string Filename { get; set; }

        public DeleteBlobCommand(string filename)
        {
            Filename = filename;
        }
    }

    public class DeleteFileCommandHandler : IRequestHandler<DeleteBlobCommand, bool>
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public DeleteFileCommandHandler(CloudBlobClient cloudBlobClient)
        {
            _cloudBlobClient = cloudBlobClient;
        }

        public async Task<bool> Handle(DeleteBlobCommand request, CancellationToken cancellationToken)
        {
            var blobContainer = _cloudBlobClient.GetContainerReference(Constants.Storage.ContainerName);
            var blob = blobContainer.GetBlockBlobReference(request.Filename);

            await blob.DeleteAsync();

            return true;
        }
    }
}
