using System;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

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
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly ILogger _logger;

        public GetBlobAsByteArrayQueryHandler(CloudBlobClient cloudBlobClient, ILogger logger)
        {
            _cloudBlobClient = cloudBlobClient;
            _logger = logger;
        }

        public async Task<ValueWrapper<byte[]>> Handle(GetBlobAsByteArrayQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var blobContainer = _cloudBlobClient.GetContainerReference(Constants.Storage.ContainerName);
                var blob = blobContainer.GetBlockBlobReference(request.Filename);

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
