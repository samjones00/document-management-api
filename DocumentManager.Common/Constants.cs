namespace DocumentManager.Common
{
    public static class Constants
    {
        //public const string BlobStorageContainer = "Uploads";

        public static class Storage
        {
            public const string ContainerName = "uploads";
            public const string ConnectionStringName = "StorageConnectionString";
            public const string BlobPath = "Uploads/{name}";
        }

        public static class Cosmos
        {
            public const string ContainerName = "Uploads";
            public const string DatabaseName = "UploadManager";
            public const string ConnectionStringName = "CosmosConnectionString";
        }
    }
}
