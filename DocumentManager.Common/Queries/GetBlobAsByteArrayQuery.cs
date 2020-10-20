using System;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace DocumentManager.Core.Queries
{
    public class GetBlobAsByteArrayQuery : IRequest<ValueWrapper<byte[]>>
    {
        public string Filename { get; set; }

        public GetBlobAsByteArrayQuery(string filename)
        {
            Filename = filename;
        }
    }

    public class GetBlobAsByteArrayQueryHandler : IRequestHandler<GetBlobAsByteArrayQuery, ValueWrapper<byte[]>>
    {
        private readonly BlobClient _cloudBlobClient;
        private readonly ILogger _logger;

        public GetBlobAsByteArrayQueryHandler(BlobClient cloudBlobClient, ILogger logger)
        {
            _cloudBlobClient = cloudBlobClient;
            _logger = logger;
        }

        public async Task<ValueWrapper<byte[]>> Handle(GetBlobAsByteArrayQuery request, CancellationToken cancellationToken)
        {
            try
            {
                    BlobContainerClient container = new BlobContainerClient(connectionString, Constants.Storage.ContainerName);

                _cloudBlobClient.DownloadAsync()

                //var blobContainer = _cloudBlobClient.(Constants.Storage.ContainerName);
                var blob = container.getblock(request.Filename);

                byte[] fileContent = new byte[blob.StreamWriteSizeInBytes];

                await blob.DownloadToByteArrayAsync(fileContent, 0);

                return new ValueWrapper<byte[]>(fileContent, true);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred", ex);
                return new ValueWrapper<byte[]>(null, false);
            }
        }
    }
}
