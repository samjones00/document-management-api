using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace DocumentManager.Core.Commands
{
    public class DeleteDocumentCommand : IRequest<bool>
    {
        public string Filename { get; set; }

        public DeleteDocumentCommand(string filename)
        {
            Filename = filename;
        }
    }

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
    {
        private readonly CosmosClient _client;
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;

        public DeleteDocumentCommandHandler(CosmosClient client, ILogger<DeleteDocumentCommandHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
                var queryDefinition = new QueryDefinition("select * from c");
                var queryResultSetIterator = container.GetItemQueryIterator<Document>(queryDefinition);
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                var doc = currentResultSet.FirstOrDefault(x => x.Filename == request.Filename);

                await container.DeleteItemAsync<Document>(doc.Id, new PartitionKey(doc.ContentType), null, cancellationToken);

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
