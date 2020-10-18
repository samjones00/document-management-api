using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using DocumentManager.Core.Models;
using DocumentManager.Core.Queries;
using Microsoft.Azure.Cosmos;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Queries
{
    public class ListDocumentsQueryTests
    {
        [Fact]
        public void Handle_ShouldReturnCollection()
        {
            //var request = new ListDocumentsQuery
            //{
            //    SortDirection = ListSortDirection.Ascending,
            //    SortProperty = "Id"
            //};

            //var client = new Mock<CosmosClient>();
            //var cancellationToken = new CancellationToken();
            //var handler = new ListDocumentsQueryHandler(client.Object);

            //var response = handler.Handle(request, cancellationToken);

            //response.ShouldNotBeNull();
            //response.ShouldBeOfType<List<Document>>();
        }
    }
}
