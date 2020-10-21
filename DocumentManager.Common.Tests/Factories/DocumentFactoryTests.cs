using System;
using DocumentManager.Core.Factories;
using DocumentManager.Core.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Factories
{
    public class DocumentFactoryTests
    {
        [Fact]
        public void Create_GivenFilenameAndSize_ReturnsDocumentObjectValue()
        {
            var filename = "example.pdf";
            var bytes = 2048;

            var dateCreated = new DateTime(2000,12,31,01,02,03);
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(dateCreated);

            var uploadItemFactory = new DocumentFactory(dateTimeProvider.Object);

            var result = uploadItemFactory.Create(filename, bytes);

            result.IsSuccessful.ShouldBeTrue();
            result.Value.Bytes.ShouldBe(2048);
            result.Value.Filename.ShouldBe(filename);
            result.Value.ContentType.ShouldBe("application/pdf");
            result.Value.Id.ShouldBeOfType<string>();
            result.Value.Id.ShouldNotBe(string.Empty);
            result.Value.DateCreated.ShouldBe(dateCreated);
        }

        [Fact]
        public void Create_GivenFilenameWithoutExtension_ReturnsDocumentObjectValue()
        {
            var filename = "example";
            var bytes = 2048;
            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(dateCreated);

            var uploadItemFactory = new DocumentFactory(dateTimeProvider.Object);

            var result = uploadItemFactory.Create(filename, bytes);

            result.IsSuccessful.ShouldBeTrue();
            result.Value.Bytes.ShouldBe(2048);
            result.Value.Filename.ShouldBe(filename);
            result.Value.ContentType.ShouldBe("application/octet-stream");
            result.Value.Id.ShouldBeOfType<string>();
            result.Value.Id.ShouldNotBe(string.Empty);
            result.Value.DateCreated.ShouldBe(dateCreated);
        }

        [Fact]
        public void Create_GivenEmptyFilename_ReturnsNullObjectValue()
        {
            var filename = string.Empty;
            var bytes = 2048;
            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(dateCreated);

            var uploadItemFactory = new DocumentFactory(dateTimeProvider.Object);

            var result = uploadItemFactory.Create(filename, bytes);

            result.IsSuccessful.ShouldBeFalse();
            result.Value.ShouldBeNull();
        }

        [Fact]
        public void Create_GivenEmptyBytes_ReturnsNullObjectValue()
        {
            var filename = "example.pdf";
            var bytes = 0;
            var dateCreated = new DateTime(2000, 12, 31, 01, 02, 03);

            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(dateCreated);

            var uploadItemFactory = new DocumentFactory(dateTimeProvider.Object);

            var result = uploadItemFactory.Create(filename, bytes);

            result.IsSuccessful.ShouldBeFalse();
            result.Value.ShouldBeNull();
        }
    }
}
