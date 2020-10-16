using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentManager.Common.Commands
{
    public class UploadDocumentCommand : IRequest<bool>
    {
        public byte[] Bytes { get; set; }
        public string Filename { get; set; }

        public UploadDocumentCommand(string filename, byte[] bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, bool>
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public UploadDocumentCommandHandler(IConfiguration configuration)
        {
            var storageConnectionString = configuration.GetConnectionString(Constants.Storage.ConnectionStringName);

            CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount);
            _cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<bool> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
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
