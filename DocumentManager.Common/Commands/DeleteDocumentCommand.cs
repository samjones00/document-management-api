using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Models;
using MediatR;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentManager.Common.Commands
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

        public DeleteDocumentCommandHandler(IResolver<CosmosClient> resolver)
        {
            _cosmosClient = resolver.Resolve();
        }

        public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var container = _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            QueryDefinition queryDefinition = new QueryDefinition("select * from c");
            var queryResultSetIterator = container.GetItemQueryIterator<Document>(queryDefinition);

            FeedResponse<Document> currentResultSet = await queryResultSetIterator.ReadNextAsync();

            var doc = currentResultSet.FirstOrDefault(x => x.Filename == request.Filename);

            await container.DeleteItemAsync<Document>(doc.Id, new PartitionKey(doc.ContentType));

            return true;
        }
    }

}
