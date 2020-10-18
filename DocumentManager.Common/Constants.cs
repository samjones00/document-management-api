namespace DocumentManager.Core
{
    public static class Constants
    {
        public static class Storage
        {
            public const string ContainerName = "uploads";
        }

        public static class Cosmos
        {
            public const string ContainerName = "Uploads";
            public const string DatabaseName = "UploadManager";
            public const string ConnectionStringName = "CosmosConnectionString";
        }
    }
}
