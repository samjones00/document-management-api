﻿namespace DocumentManager.Core
{
    public static class Constants
    {
        public static class Storage
        {
            public const string ContainerName = "uploads";
            public const string ConnectionStringName = "AzureWebJobsStorage";
        }

        public static class Cosmos
        {
            public const string ContainerName = "uploads";
            public const string DatabaseName = "document-manager";
            public const string ConnectionStringName = "CosmosConnectionString";
        }

        public static class ValidatorSettings
        {
            public const string SectionName = "Validator";
            public const string MaximumFileSizeInBytes = "MaximumFileSizeInBytes";
            public const string AllowedContentTypes = "AllowedContentTypes";
        }
    }
}
