using System;
using System.Net.Http;
using System.Reflection;
using Azure.Storage.Blobs;
using DocumentManager.Api;
using DocumentManager.Core;
using DocumentManager.Core.Factories;
using DocumentManager.Core.Infrastructure;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using DocumentManager.Core.Providers;
using DocumentManager.Core.Repositories;
using DocumentManager.Core.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

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
            builder.Services.AddSingleton<IDocumentFactory, DocumentFactory>();
            builder.Services.AddSingleton<IValidator<UploadRequest>, UploadRequestValidator>();
            builder.Services.AddTransient<IDocumentRepository, CosmosRepository>();
            builder.Services.AddTransient<IStorageRepository, BlobStorageRepository>();

            ConfigureLogging(builder);
            ConfigureCosmosClient(builder, localConfig);
            ConfigureBlobClient(builder, localConfig);
        }

        private void ConfigureCosmosClient(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
            {
                HttpClientFactory = () =>
                {
                    HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    return new HttpClient(httpMessageHandler);
                },
                ConnectionMode = ConnectionMode.Gateway
            };

            var cosmosClient =
                new CosmosClient(configuration.GetConnectionString(Constants.Cosmos.ConnectionStringName),
                    cosmosClientOptions);

            _ = new SetupCosmos(cosmosClient).Setup();

            builder.Services.AddSingleton(_ => cosmosClient);
        }

        private void ConfigureBlobClient(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            static string GenerateConnStr(string ip = "127.0.0.1", int blobport = 10000, int queueport = 10001, int tableport = 10002)
            {
                return $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://{ip}:{blobport}/devstoreaccount1;TableEndpoint=http://{ip}:{tableport}/devstoreaccount1;QueueEndpoint=http://{ip}:{queueport}/devstoreaccount1;";
            }

            builder.Services.AddSingleton(_ =>
                new BlobContainerClient(GenerateConnStr(), Constants.Storage.ContainerName));
        }

        private void ConfigureLogging(IFunctionsHostBuilder builder)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(@"Logs\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Services.AddLogging(lb => lb.AddSerilog(logger));
        }
    }
}