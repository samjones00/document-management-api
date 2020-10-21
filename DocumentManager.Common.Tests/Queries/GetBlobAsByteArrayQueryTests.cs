using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using DocumentManager.Core.Queries;
using Microsoft.Extensions.Logging;

using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Queries
{
    public class GetBlobAsByteArrayQueryTests
    {
        //[Fact]
        //public async Task Handler()
        //{
        //    var storageAccount = new Mock<CloudStorageAccount>();
        //   // CloudStorageAccount.TryParse(It.IsAny<string>(), out var storageAccount);
        //    var client = storageAccount.Object.CreateCloudBlobClient();
        ////    client.GetContainerReference.
        //    //var client = new Mock<CloudBlobClientWrapper>();
        //    //client.Setup(x => x.GetContainerReference(It.IsAny<string>()))
        //    //    .Returns(new CloudBlobContainer(new StorageUri(new Uri("http://www.fakedomain.com")), new StorageCredentials()));

        //    var logger = new Mock<ILogger>();

        //    var request = new GetBlobAsByteArrayQuery("example.pdf");
        //    var handler = new GetBlobAsByteArrayQueryHandler(client, logger.Object);
        //    var cancellationToken = new CancellationToken();

        //    var response = await handler.Handle(request, cancellationToken);

        //    response.IsSuccessful.ShouldBeTrue();
        //    response.Value.ShouldNotBeNull();
        //    response.Value.ShouldBeOfType<byte[]>();
        //}
    }
}
