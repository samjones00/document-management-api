using System;
using System.Reflection;
using DocumentManager.Api;
using DocumentManager.Core;
using DocumentManager.Core.Factories;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using DocumentManager.Core.Providers;
using DocumentManager.Core.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.WindowsAzure.Storage;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DocumentManager.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var localConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("settings.json")
                .AddJsonFile("local.settings.json")
                .Build();

            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), localConfig));
            builder.Services.AddMediatR(typeof(Constants).GetTypeInfo().Assembly);
            builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            builder.Services.AddSingleton<IUploadItemFactory, DocumentFactory>();
            builder.Services.AddSingleton<IValidator<UploadRequest>, UploadRequestValidator>();

            builder.Services.AddSingleton(_ =>
                new CosmosClient(localConfig.GetConnectionString(Constants.Cosmos.ConnectionStringName)));

            builder.Services.AddSingleton(_ =>
            {
                CloudStorageAccount.TryParse(localConfig.GetValue<string>("AzureWebJobsStorage"),
                    out var storageAccount);
                return storageAccount.CreateCloudBlobClient();
            });
        }
    }
}