using System.Threading.Tasks;
using DocumentManager.Core.Models;
using Microsoft.Azure.Cosmos;

namespace DocumentManager.Core.Infrastructure
{
    public class SetupCosmos
    {
        private readonly CosmosClient _client;

        public SetupCosmos(CosmosClient client)
        {
            _client = client;
        }

        public async Task Setup()
        {
            var databaseName = Constants.Cosmos.DatabaseName;
            var container = Constants.Cosmos.ContainerName;
            var partitionKey = nameof(Document.ContentType);
            var database = await CreateDatabaseAsync(databaseName);
            await CreateContainerAsync(database, container, partitionKey);
        }

        private async Task<Database> CreateDatabaseAsync(string databaseName)
        {
            return await _client.CreateDatabaseIfNotExistsAsync(databaseName);
        }

        private async Task<Container> CreateContainerAsync(Database database, string containerId, string partitionKey)
        {
            return await database.CreateContainerIfNotExistsAsync(containerId, $"/{partitionKey}");
        }
    }
}
