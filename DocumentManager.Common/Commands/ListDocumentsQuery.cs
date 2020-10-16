using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Models;
using MediatR;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentManager.Common.Commands
{
    public class ListDocumentsQuery : IRequest<IOrderedQueryable<Document>>
    {
    }

    public class ListDocumentsQueryHandler : IRequestHandler<ListDocumentsQuery, IOrderedQueryable<Document>>
    {
        private readonly CosmosClient _cosmosClient;

        public ListDocumentsQueryHandler(IResolver<CosmosClient> resolver)
        {
            _cosmosClient = resolver.Resolve();
        }

        public Task<IOrderedQueryable<Document>> Handle(ListDocumentsQuery request, CancellationToken cancellationToken)
        {
            var container = _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);

            var list = container.GetItemLinqQueryable<Document>(true);

            return (Task<IOrderedQueryable<Document>>)list;
        }
    }
}
