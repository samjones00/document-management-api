using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentManager.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocumentManager.Core.Repositories
{
    public class BlobStorageRepository: IStorageRepository
    {
        private readonly BlobContainerClient _client;
        private readonly ILogger<BlobStorageRepository> _logger;

        public BlobStorageRepository(BlobContainerClient client, ILogger<BlobStorageRepository> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task Delete(string filename, CancellationToken cancellationToken)
        {
            try
            {
                var blockBlob = _client.GetBlobClient(filename);

                await blockBlob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred", ex);
            }
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
            BlobDownloadInfo download = await blockBlob.DownloadAsync(cancellationToken);

            using (var stream = new MemoryStream())
            {
                await download.Content.CopyToAsync(stream, cancellationToken);
                return stream;
            }
        }
    }
}
