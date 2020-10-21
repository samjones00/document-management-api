using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace DocumentManager.Core.Queries
{
    public class GetBlobAsByteArrayQuery : IRequest<ValueWrapper<MemoryStream>>
    {
        public string Filename { get; set; }

        public GetBlobAsByteArrayQuery(string filename)
        {
            Filename = filename;
        }
    }

    public class GetBlobAsByteArrayQueryHandler : IRequestHandler<GetBlobAsByteArrayQuery, ValueWrapper<MemoryStream>>
    {
        private readonly BlobContainerClient _client;
        private readonly ILogger<GetBlobAsByteArrayQueryHandler> _logger;

        public GetBlobAsByteArrayQueryHandler(BlobContainerClient client, ILogger<GetBlobAsByteArrayQueryHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ValueWrapper<MemoryStream>> Handle(GetBlobAsByteArrayQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var blockBlob = _client.GetBlobClient(request.Filename);
                BlobDownloadInfo download = await blockBlob.DownloadAsync(cancellationToken);

                using (var stream = new MemoryStream())
                {
                    await download.Content.CopyToAsync(stream, cancellationToken);
                    return new ValueWrapper<MemoryStream>(stream, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred", ex);
                return new ValueWrapper<MemoryStream>(null, false);
            }
        }
    }
}
