using System;
using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Providers;
using Moq;
using Newtonsoft.Json.Converters;
using Shouldly;
using Xunit;

namespace DocumentManager.Common.Tests
{
    public class UploadItemFactoryTests
    {
        [Fact]
        public void Create_GivenFilenameAndSize_ReturnsObject()
        {
            var filename = "example.pdf";
            var bytes = 2048;
            var dateCreated = new DateTime(2000,12,31,01,02,03);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow()).Returns(dateCreated);

            var uploadItemFactory = new UploadItemFactory(dateTimeProvider.Object);


            var result = uploadItemFactory.Create(filename, bytes);

            result.Bytes.ShouldBe(2048);
            result.Filename.ShouldBe(filename);
            result.ContentType.ShouldBe("application/pdf");
            result.Id.ShouldBeOfType<Guid>();
            result.Id.ShouldNotBe(Guid.Empty);
            result.DateCreated.ShouldBe(dateCreated);
        }

        public void Create_GivenEmptyFilenameOrSize_ReturnsObject()
        {
            var filename = "example.pdf";
            var bytes = 2048;
            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow()).Returns(dateCreated);

            var uploadItemFactory = new UploadItemFactory(dateTimeProvider.Object);


            var result = uploadItemFactory.Create(filename, bytes);

            result.Bytes.ShouldBe(2048);
            result.Filename.ShouldBe(filename);
            result.ContentType.ShouldBe("application/pdf");
            result.Id.ShouldBeOfType<Guid>();
            result.Id.ShouldNotBe(Guid.Empty);
            result.DateCreated.ShouldBe(dateCreated);
        }
    }
}
