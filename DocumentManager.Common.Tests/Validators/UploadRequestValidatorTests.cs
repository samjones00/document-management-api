using DocumentManager.Core.Models;
using DocumentManager.Core.Tests.Helpers;
using DocumentManager.Core.Validators;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Validators
{
    public class UploadRequestValidatorTests
    {
        public readonly int MaximumFileSizeInBytes = 5242880;

        [Fact]
        public void Validate_GivenAllowedContentType_ShouldReturnValid()
        {
            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);

            var request = new UploadRequest
            {
                Filename = "example.pdf",
                Bytes = new[] {new byte(), new byte(), new byte()}
            };

            var result = validator.Validate(request);

            result.IsValid.ShouldBe(true);
            result.Errors.ShouldBeEmpty();
        }

        [Fact]
        public void Validate_GivenNotAllowedContentType_ShouldReturnInvalid()
        {
            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);

            var request = new UploadRequest
            {
                Filename = "example.csv",
                Bytes = new byte[1000]
            };

            var result = validator.Validate(request);

            result.IsValid.ShouldBe(false);
            result.Errors.ShouldContain(x => x.ErrorMessage == "The file must be a valid content type");
        }

        [Fact]
        public void Validate_GivenAllowedContentTypeAndExceedingSize_ShouldReturnInvalid()
        {
            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, MaximumFileSizeInBytes*2);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);

            var request = new UploadRequest
            {
                Filename = "example.csv",
                Bytes = new byte[MaximumFileSizeInBytes + 1]
            };

            var result = validator.Validate(request);

            result.IsValid.ShouldBe(false);
            result.Errors.ShouldContain(x => x.ErrorMessage == "The file must be a valid size");
        }

        [Fact]
        public void Validate_GivenAllowedContentTypeAndNoFilename_ShouldReturnInvalid()
        {
            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration,"application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);

            var request = new UploadRequest
            {
                Filename = string.Empty,
                Bytes = new byte[1000]
            };

            var result = validator.Validate(request);

            result.IsValid.ShouldBe(false);
            result.Errors.ShouldContain(x => x.ErrorMessage == "You must provide a filename");
        }

        [Fact]
        public void Validate_GivenAllowedContentTypeAndZeroBytes_ShouldReturnInvalid()
        {
            var configuration = new Mock<IConfiguration>();
            ConfigurationHelper.SetupMaximumFileSizeInBytes(configuration, MaximumFileSizeInBytes);
            ConfigurationHelper.SetupAllowedContentTypes(configuration, "application/pdf");

            var validator = new UploadRequestValidator(configuration.Object);

            var request = new UploadRequest
            {
                Filename = "example.pdf",
                Bytes = new byte[0]
            };

            var result = validator.Validate(request);

            result.IsValid.ShouldBe(false);
            result.Errors.ShouldContain(x => x.ErrorMessage == "You must provide a file");
        }
    }
}