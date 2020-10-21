using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DocumentManager.Core.Commands
{
    public class DeleteBlobCommand : IRequest<bool>
    {
        public string Filename { get; set; }

        public DeleteBlobCommand(string filename)
        {
            Filename = filename;
        }
    }

    public class DeleteFileCommandHandler : IRequestHandler<DeleteBlobCommand, bool>
    {
        private readonly BlobContainerClient _client;
        private readonly ILogger<DeleteFileCommandHandler> _logger;

        public DeleteFileCommandHandler(BlobContainerClient client, ILogger<DeleteFileCommandHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteBlobCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var blockBlob = _client.GetBlobClient(request.Filename);

                await blockBlob.DeleteIfExistsAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred", ex);
                return false;
            }
        }
    }
}
