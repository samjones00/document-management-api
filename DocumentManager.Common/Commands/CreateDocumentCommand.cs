using System;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace DocumentManager.Core.Commands
{
    public class CreateDocumentCommand : IRequest<bool>
    {
        public string Filename { get; set; }
        public long Bytes { get; set; }

        public CreateDocumentCommand(string filename, long bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, bool>
    {
        private readonly IDocumentFactory _documentFactory;
        private readonly ILogger _logger;
        private readonly CosmosClient _cosmosClient;

        public CreateDocumentCommandHandler(CosmosClient cosmosClient, IDocumentFactory documentFactory,ILogger logger)
        {
            _documentFactory = documentFactory;
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        public async Task<bool> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = _documentFactory.Create(request.Filename, request.Bytes);

                if (!document.IsSuccessful)
                {
                    return false;
                }

                var container =
                    _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);

                await container.CreateItemAsync(document.Value, null, null, cancellationToken);

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
