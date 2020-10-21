using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using MediatR;
using Microsoft.Extensions.Logging;

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
        private readonly BlobContainerClient _client;
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;

        public UploadFileCommandHandler(BlobContainerClient client, ILogger<DeleteDocumentCommandHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<bool> Handle(UploadBlobCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var blobClient = _client.GetBlobClient(request.Filename);
                var stream = new MemoryStream(request.Bytes);

                await blobClient.UploadAsync(stream, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred", ex);
                return false;
            }

            return true;
        }
    }
}
