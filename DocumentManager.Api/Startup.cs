using System.Reflection;
using DocumentManager.Api;
using DocumentManager.Common;
using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Providers;
using MediatR;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Blob;

[assembly: FunctionsStartup(typeof(Startup))]

namespace DocumentManager.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();

            //builder.Services.AddSingleton<IMyService>((s) => {
            //    return new MyService();
            //});
            builder.Services.AddMediatR(typeof(DocumentManager.Common.Constants).GetTypeInfo().Assembly);
            builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            builder.Services.AddSingleton<IUploadItemFactory, UploadItemFactory>();
            builder.Services.AddSingleton<IResolver<CloudBlobClient>, CloudBlobClientResolver>();
            builder.Services.AddSingleton<IResolver<CosmosClient>, CosmosClientResolver>();
        }
    }
}