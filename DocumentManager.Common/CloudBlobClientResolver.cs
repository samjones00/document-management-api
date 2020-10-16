using DocumentManager.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentManager.Common
{
    public class CloudBlobClientResolver : IResolver<CloudBlobClient>
    {
        private readonly IConfiguration _configuration;

        public CloudBlobClientResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CloudBlobClient Resolve()
        {
            var connectionString = _configuration.GetConnectionString(Constants.Storage.ConnectionStringName);

            CloudStorageAccount.TryParse(connectionString, out var storageAccount);
            return storageAccount.CreateCloudBlobClient();
        }
    }
}
