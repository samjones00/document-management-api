using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentManager.Core.Commands
{
    public class UploadBlobCommand : IRequest<bool>
    {
        public byte[] Bytes { get; set; }
        public string Filename { get; set; }

        public UploadBlobCommand(string filename, byte[] bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class UploadFileCommandHandler : IRequestHandler<UploadBlobCommand, bool>
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public UploadFileCommandHandler(CloudBlobClient cloudBlobClient)
        {
            _cloudBlobClient = cloudBlobClient;
        }

        public async Task<bool> Handle(UploadBlobCommand request, CancellationToken cancellationToken)
        {
            var container = _cloudBlobClient.GetContainerReference(Constants.Storage.ContainerName);
            _ = await container.CreateIfNotExistsAsync();
            var filename = request.Filename;
            var cloudBlockBlob = container.GetBlockBlobReference(filename);

            await cloudBlockBlob.UploadFromStreamAsync(new MemoryStream(request.Bytes));

            return true;
        }
    }
}
