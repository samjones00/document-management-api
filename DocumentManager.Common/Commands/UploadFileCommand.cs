using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Common.Interfaces;
using MediatR;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentManager.Common.Commands
{
    public class UploadFileCommand : IRequest<bool>
    {
        public byte[] Bytes { get; set; }
        public string Filename { get; set; }

        public UploadFileCommand(string filename, byte[] bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, bool>
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public UploadFileCommandHandler(IResolver<CloudBlobClient> resolver)
        {
            _cloudBlobClient = resolver.Resolve();
        }

        public async Task<bool> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var container = _cloudBlobClient.GetContainerReference(Constants.Storage.ContainerName);
            await container.CreateIfNotExistsAsync();
            var filename = request.Filename;

            var cloudBlockBlob = container.GetBlockBlobReference(filename);

            await cloudBlockBlob.UploadFromStreamAsync(new MemoryStream(request.Bytes));

            return true;
        }
    }
}
