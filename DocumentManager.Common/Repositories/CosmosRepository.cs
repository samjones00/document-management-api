using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using DocumentManager.Core.Queries;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace DocumentManager.Core.Repositories
{
    public class CosmosRepository: IDocumentRepository
    {
        private readonly CosmosClient _client;
        private readonly ILogger<ListDocumentsQueryHandler> _logger;

        public CosmosRepository(CosmosClient client, ILogger<ListDocumentsQueryHandler> logger)
        {
            _client = client;
            _logger = logger;
        }
        
        private string GenerateQuery(string sortProperty)
        {
            var isValidProperty = Enum.TryParse<SortProperty>(sortProperty, true, out var sortPropertyEnum);

            return isValidProperty ? $"select * from c order by c.{sortPropertyEnum}" : "select * from c";
        }

        public async Task<IEnumerable<Document>> Get(Expression<Func<Document, bool>> Query, string sortProperty, CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition(GenerateQuery(sortProperty));

            var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            //var queryDefinition = request.IsSortedQuery
            //    ? new QueryDefinition("select * from c order by c.Filename where Filename = '@direction'")
            //        .WithParameter("@property", request.SortProperty)
            //        .WithParameter("@direction", request.SortDirection)
            //    : new QueryDefinition("select * from c");

            var results = new List<Document>();

            //QueryDefinition query2 = new QueryDefinition(
            //        "select * from t where t.Account = @account")
            //    .WithParameter("@account", "12345");

            //var a = document => document.Filename

            //var param = "Address";
            //var pi = typeof(Document).GetProperty(request.SortProperty);
            //var orderByAddress = items.OrderBy(x => pi.GetValue(x, null));

            //using (FeedIterator<Document> setIterator = container.GetItemLinqQueryable<Document>()
            //    //.Where(request.Query)
            //    .OrderBy(s => s.GetType().GetProperty(request.SortProperty).GetValue(s))

            //    .ToFeedIterator())
            //{
            //    while (setIterator.HasMoreResults)
            //    {

            //        var response = await setIterator.ReadNextAsync(cancellationToken);

            //        results.AddRange(response.ToList());
            //    }
            //}

            //return new ValueWrapper<List<Document>>(results, true);

            //var query1 = container
            //    .GetItemLinqQueryable<Document>()
            //    .Where(x => x.Id == "apple-iphone")
            //    .OrderBy(x => x);
            //System.Linq.Expressions.Expression<Func<IEnumerable<Document>, bool>> e = x => x.Where(d => d.Id == f);

            //var a = new Func<IEnumerable<Document>, bool>(document => document.Where(x => x.Filename.Equals("hi")))

            // QueryDefinition queryDefinition2 = request.ToQueryDefinition();

            var isValidProperty = Enum.TryParse<SortProperty>(sortProperty, true, out var sortPropertyEnum);

            Func<IEnumerable<Document>, IOrderedEnumerable<Document>> a = (x => x.OrderBy(x => x.Bytes));


            switch (sortPropertyEnum)
            {
                case SortProperty.Bytes:
                    a = documents => documents.OrderBy(x => x.Bytes);
                    break;
                default:
                    break;
            }




            //if (request.Query != null)
            //{
            var setIterator = container.GetItemLinqQueryable<Document>()
                .Where(Query)
                .OrderBy(x => a)
                .ToFeedIterator();


            //}

            //var query = container.GetItemQueryIterator<Document>(queryDefinition);

            while (setIterator.HasMoreResults)
            {
                var response = await setIterator.ReadNextAsync(cancellationToken);

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task Delete(string filename, CancellationToken cancellationToken)
        {
            var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            var queryDefinition = new QueryDefinition("select * from c");
            var queryResultSetIterator = container.GetItemQueryIterator<Document>(queryDefinition);
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            var doc = currentResultSet.FirstOrDefault(x => x.Filename == filename);

            await container.DeleteItemAsync<Document>(doc.Id, new PartitionKey(doc.ContentType), null, cancellationToken);
        }

        public async Task Add(Document document, CancellationToken cancellationToken)
        {
            var container =
                _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);

            await container.CreateItemAsync(document, null, null, cancellationToken);
        }

    }
}
