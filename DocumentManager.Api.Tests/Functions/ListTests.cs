using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using DocumentManager.Core.Queries;
using DocumentManager.Core.Tests;
using DocumentManager.Core.Tests.Helpers;
using DocumentManager.Core.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Api.Tests.Functions
{
    public class ListTests
    {
        [Fact]
        public async Task List_GivenNoParametersProvided_ShouldReturnDocumentCollection()
        {
            var sortProperty = "Filename";
            var sortDirection = "Ascending";

            var expectedResults = new List<Document>
            {
                new Document("example1.pdf", 1000, "application/pdf", new DateTime(02, 03, 04, 05, 06, 07)),
                new Document("example2.pdf", 1000, "application/pdf", new DateTime(01, 02, 03, 04, 05, 06)),
                new Document("example3.pdf", 1000, "application/pdf", new DateTime(08, 06, 05, 04, 03, 02)),
            };

            var expectedContentResult = new CreatedResult("/", expectedResults);

            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, Constants.MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);
            var logger = new TestLogger<AzureFunctions>();
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(m => m.Send(It.IsAny<GetDocumentCollectionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValueWrapper<IEnumerable<Document>>(new List<Document>(), true));

            var function = new AzureFunctions(mediator.Object, validator, logger);

            var httpRequest = HttpRequestHelper.CreateMockRequest(expectedResults);
            var actionResult = await function.List(httpRequest.Object, sortProperty, sortDirection);
            var statusCodeResult = (IStatusCodeActionResult) actionResult;

            var createdResult = (OkObjectResult) actionResult;

            createdResult.Value.ShouldNotBeSameAs(expectedContentResult.Value);

            statusCodeResult.StatusCode.ShouldBe((int) HttpStatusCode.OK);

            var msg = logger.Logs[0];
            Assert.Contains("documents found", msg);
        }
    }
}
