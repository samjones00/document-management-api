using System;
using System.Reflection;
using Azure.Storage.Blobs;
using DocumentManager.Api;
using DocumentManager.Core;
using DocumentManager.Core.Factories;
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
            ConfigureClients(builder, localConfig);
        }

        private void ConfigureClients(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton(_ =>
                new CosmosClient(configuration.GetConnectionString(Constants.Cosmos.ConnectionStringName)));

            builder.Services.AddSingleton(_ =>
                new BlobContainerClient(configuration.GetValue<string>(Constants.Storage.ConnectionStringName),
                    Constants.Storage.ContainerName));
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