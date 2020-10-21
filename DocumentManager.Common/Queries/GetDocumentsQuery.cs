using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace DocumentManager.Core.Queries
{
    public class GetDocumentsQuery : IRequest<ValueWrapper<List<Document>>>
    {
        public Expression<Func<Document, bool>> Query { get; set; }
        public string SortProperty { get; set; }
        public ListSortDirection SortDirection { get; set; }
        public bool IsSortedQuery => !string.IsNullOrEmpty(SortProperty);

        public GetDocumentsQuery(Expression<Func<Document, bool>> query)
        {
            Query = query;
        }

        public GetDocumentsQuery(string sortProperty, ListSortDirection sortDirection)
        {
            SortProperty = sortProperty;
            SortDirection = sortDirection;
            Query = x => x != null;
        }
    }

    public class ListDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery,ValueWrapper<List<Document>>>
    {
        private readonly CosmosClient _client;
        private readonly ILogger<ListDocumentsQueryHandler> _logger;

        public ListDocumentsQueryHandler(CosmosClient client, ILogger<ListDocumentsQueryHandler> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ValueWrapper<List<Document>>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
                var queryDefinition = request.IsSortedQuery
                    ? new QueryDefinition("select * from c order by c.@property")
                        .WithParameter("@property", request.SortProperty)
                        .WithParameter("@direction", request.SortDirection)
                    : new QueryDefinition("select * from c");

                // QueryDefinition queryDefinition2 = request.ToQueryDefinition();


                var results = new List<Document>();

                //var a = document => document.Filename

                //var param = "Address";
                // var pi = typeof(Document).GetProperty(request.SortProperty);
                //var orderByAddress = items.OrderBy(x => pi.GetValue(x, null));

                using (FeedIterator<Document> setIterator = container.GetItemLinqQueryable<Document>()
                    .Where(request.Query)
                    //  .OrderBy(x => pi.GetValue(x, null))
                    .ToFeedIterator())
                {
                    //Asynchronous query execution
                    while (setIterator.HasMoreResults)
                    {
                        //foreach (var item in await setIterator.ReadNextAsync())
                        //{
                        //    {
                        //        Console.WriteLine(item.cost);
                        //    }
                        //}

                        var response = await setIterator.ReadNextAsync(cancellationToken);

                        results.AddRange(response.ToList());
                    }
                }

                return new ValueWrapper<List<Document>>(results, true);

                //var query1 = container
                //    .GetItemLinqQueryable<Document>()
                //    .Where(x => x.Id == "apple-iphone")
                //    .OrderBy(x => x);
                // System.Linq.Expressions.Expression<Func<IEnumerable<Document>,bool>> e = x => x.Where(d=>d.Id==f);

                // var a = new Func<IEnumerable<Document>, bool>(document => document.Where(x => x.Filename.Equals("hi")))

                // QueryDefinition queryDefinition2 = request.ToQueryDefinition();


                //var query = container.GetItemQueryIterator<Document>(queryDefinition);

                //while (query.HasMoreResults)
                //{
                //    var response = await query.ReadNextAsync(cancellationToken);

                //    results.AddRange(response.ToList());
                //}

                //return results;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred", ex);

                return new ValueWrapper<List<Document>>(null, false);
            }
        }
    }
}
