using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Common.Interfaces;
using MediatR;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentManager.Common.Commands
{
    public class DeleteFileCommand : IRequest<bool>
    {
        public string Filename { get; set; }

        public DeleteFileCommand(string filename)
        {
            Filename = filename;
        }
    }

    public class DeleteFileCommandHandler : IRequestHandler<UploadFileCommand, bool>
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public DeleteFileCommandHandler(IResolver<CloudBlobClient> resolver)
        {
            _cloudBlobClient = resolver.Resolve();
        }

        public async Task<bool> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var blobContainer = _cloudBlobClient.GetContainerReference(Constants.Storage.ContainerName);

            var blob = blobContainer.GetBlockBlobReference(request.Filename);

            await blob.DeleteAsync();

            return true;
        }
    }
}
