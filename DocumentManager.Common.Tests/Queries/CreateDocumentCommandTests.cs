using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Commands;
using DocumentManager.Core.Factories;
using DocumentManager.Core.Models;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace DocumentManager.Core.Tests.Queries
{
    public class CreateDocumentCommandTests
    {
        [Fact]
        public async Task Handler()
        {
            //List<Document> results = new List<Document>();
            //{
               
            //};
            //CosmosClient _mockClient = new CosmosClient();
            //_mockClient.Setup

            //Mock<FeedResponse<Document>> mockedResponse = new Mock<FeedResponse<Document>>();
            //mockedResponse.Setup(r => r.Resource).Returns(results);
            //Mock<FeedIterator<Document>> mockedIterator = new Mock<FeedIterator<Document>>();
            //mockedIterator.Setup(q => q.ReadNextAsync(It.IsAny<CancellationToken>()))
            //    .ReturnsAsync(() => mockedResponse.Object);
            //mockedIterator.SetupSequence(q => q.HasMoreResults)
            //    .Returns(true)
            //    .Returns(false);

            //while (mockedIterator.Object.HasMoreResults)
            //{
            //    FeedResponse<Document> feedresponse = await mockedIterator.Object.ReadNextAsync();
            //    Assert.Equal(results, feedresponse.Resource);
            //}

            //Mock.Get(mockedIterator.Object)
            //    .Verify(q => q.ReadNextAsync(It.IsAny<CancellationToken>()), Times.Once);
            //Mock.Get(mockedIterator.Object)
            //    .Verify(q => q.HasMoreResults, Times.Exactly(2));
            //Mock.Get(mockedResponse.Object)
            //    .Verify(r => r.Resource, Times.Once);

            ////Mock<CosmosDatabases> mockDatabases = new Mock<CosmosDatabases>();
            ////Mock<CosmosClient> client = new CosmosClient();
            ////  client.Setup(x => x.Databases).Returns(mockDatabases.Object);

            ////CosmosClient client = mockClient.Object;
            ////Assert.IsNotNull(client.Databases);

            //var logger = new Mock<ILogger>();

            ////var dateTimeProvider = new Mock<IDateTimeProvider>();
            //var uploadItemFactory = new Mock<DocumentFactory>();
            //var request = new CreateDocumentCommand("example.pdf",12345);
            //var handler = new CreateDocumentCommandHandler(_mockClient.Object, uploadItemFactory.Object, logger.Object);
            //var cancellationToken = new CancellationToken();
            //var response = await handler.Handle(request, cancellationToken);

            //response.ShouldBeTrue();
        }
    }
}
