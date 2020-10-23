using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Extensions;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using Microsoft.Azure.Cosmos;

namespace DocumentManager.Core.Repositories
{
    public class CosmosRepository : IDocumentRepository
    {
        private readonly CosmosClient _client;

        public CosmosRepository(CosmosClient client)
        {
            _client = client;
        }

        public IEnumerable<Document> GetCollection(string sortProperty, string sortDirection, CancellationToken cancellationToken)
        {
            var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);

            Func<Document, object> sortQuery = document => document.DateCreated;

            var isValidProperty = Enum.TryParse<SortProperty>(sortProperty, true, out var sortPropertyEnum);
            Enum.TryParse<ListSortDirection>(sortDirection, true, out var sortDirectionEnum);

            var isAscending = sortDirectionEnum == ListSortDirection.Ascending;

            if (isValidProperty)
            {
                sortQuery = sortPropertyEnum switch
                {
                    SortProperty.Bytes => document => document.Bytes,
                    SortProperty.Filename => document => document.Filename,
                    SortProperty.ContentType => document => document.ContentType,
                    _ => sortQuery
                };
            }

            var results = container.GetItemLinqQueryable<Document>(true)
                .OrderByWithDirection(sortQuery,!isAscending)
                .AsEnumerable();

            return results;
        }

        public Document GetSingle(string filename)
        {
            var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);

            var document = container.GetItemLinqQueryable<Document>(true)
                .Where((d, i) => d.Filename.Equals(filename, StringComparison.InvariantCultureIgnoreCase))
                .AsEnumerable()
                .FirstOrDefault();

            return document;
        }

        public async Task<bool> Delete(string filename, CancellationToken cancellationToken)
        {
            var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            var queryDefinition = new QueryDefinition("select * from c");
            var queryResultSetIterator = container.GetItemQueryIterator<Document>(queryDefinition);
            var currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);
            var doc = currentResultSet.FirstOrDefault(x => x.Filename == filename);

            if (doc == null) 
                return false;

            await container.DeleteItemAsync<Document>(doc.Id, new PartitionKey(doc.ContentType), null, cancellationToken);

            return true;
        }

        public async Task Add(Document document, CancellationToken cancellationToken)
        {
            var container = _client.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);

            await container.CreateItemAsync(document, cancellationToken: cancellationToken);
        }
    }
}
