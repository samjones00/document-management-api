using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Azure.Cosmos;

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
        private readonly CosmosClient _cosmosClient;

        public DeleteDocumentCommandHandler(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var container = _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            var queryDefinition = new QueryDefinition("select * from c");
            var queryResultSetIterator = container.GetItemQueryIterator<Document>(queryDefinition);
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            var doc = currentResultSet.FirstOrDefault(x => x.Filename == request.Filename);

            await container.DeleteItemAsync<Document>(doc.Id, new PartitionKey(doc.ContentType),null, cancellationToken);

            return true;
        }
    }

}
