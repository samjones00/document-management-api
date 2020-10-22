using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace DocumentManager.Core.Queries
{
    public class GetDocumentCollectionQuery : IRequest<ValueWrapper<List<Document>>>
    {
        public Expression<Func<Document, bool>> Query { get; }
        public string SortProperty { get; }

        public GetDocumentCollectionQuery(Expression<Func<Document, bool>> query)
        {
            Query = query;
        }

        public GetDocumentCollectionQuery(string sortProperty)
        {
            SortProperty = sortProperty;
        }
    }

    public class GetDocumentCollectionQueryHandler : IRequestHandler<GetDocumentCollectionQuery, ValueWrapper<List<Document>>>
    {
        private readonly CosmosClient _client;

        public GetDocumentCollectionQueryHandler(CosmosClient client)
        {
            _client = client;
        }

        public async Task<ValueWrapper<List<Document>>> Handle(GetDocumentCollectionQuery request,
            CancellationToken cancellationToken)
        {
            var queryDefinition = new QueryDefinition(GenerateQuery(request.SortProperty));

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

            var isValidProperty = Enum.TryParse<SortProperty>(request.SortProperty, true, out var sortPropertyEnum);

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
                .Where(request.Query)
                .OrderBy(x => a)
                .ToFeedIterator();


            //}

            //var query = container.GetItemQueryIterator<Document>(queryDefinition);

            while (setIterator.HasMoreResults)
            {
                var response = await setIterator.ReadNextAsync(cancellationToken);

                results.AddRange(response.ToList());
            }

            return new ValueWrapper<List<Document>>(results, true);
        }

        private string GenerateQuery(string sortProperty)
        {
            var isValidProperty = Enum.TryParse<SortProperty>(sortProperty, true, out var sortPropertyEnum);

            return isValidProperty ? $"select * from c order by c.{sortPropertyEnum}" : "select * from c";
        }
    }
}
