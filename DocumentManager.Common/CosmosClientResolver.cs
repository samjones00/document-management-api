using DocumentManager.Common.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace DocumentManager.Common
{
    public class CosmosClientResolver : IResolver<CosmosClient>
    {
        private readonly IConfiguration _configuration;

        public CosmosClientResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CosmosClient Resolve()
        {
            var connectionString = _configuration.GetConnectionString(Constants.Cosmos.ConnectionStringName);
            return new CosmosClient(connectionString);
        }
    }
}
