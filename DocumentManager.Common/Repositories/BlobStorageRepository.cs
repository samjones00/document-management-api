using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentManager.Core.Interfaces;

namespace DocumentManager.Core.Repositories
{
    public class BlobStorageRepository : IStorageRepository
    {
        private readonly BlobContainerClient _client;

        public BlobStorageRepository(BlobContainerClient client)
        {
            _client = client;
        }

        public async Task Delete(string filename, CancellationToken cancellationToken)
        {
            var blockBlob = _client.GetBlobClient(filename);

            await blockBlob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public async Task Add(string filename, byte[] bytes, CancellationToken cancellationToken)
        {
            var blobClient = _client.GetBlobClient(filename);
            var stream = new MemoryStream(bytes);

            await blobClient.UploadAsync(stream, cancellationToken);
        }

        public async Task<MemoryStream> Get(string filename, CancellationToken cancellationToken)
        {
            var blockBlob = _client.GetBlobClient(filename);
            BlobDownloadInfo blob = await blockBlob.DownloadAsync(cancellationToken);

            using (var stream = new MemoryStream())
            {
                await blob.Content.CopyToAsync(stream, cancellationToken);
                stream.Position = 0;
                return stream;
            }
        }
    }
}
