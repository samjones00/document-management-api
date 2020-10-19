using System.Collections.Generic;
using DocumentManager.Core.Extensions;
using DocumentManager.Core.Models;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Settings = DocumentManager.Core.Constants.ValidatorSettings;

namespace DocumentManager.Core.Validators
{
    public class UploadRequestValidator : AbstractValidator<UploadRequest>
    {
        private readonly int _maximumFileSizeInBytes;
        private readonly List<string> _allowedContentTypes;

        public UploadRequestValidator(IConfiguration configuration)
        {
            _maximumFileSizeInBytes = configuration.GetSection($"{Settings.SectionName}:{Settings.MaximumFileSizeInBytes}").Get<int>();
            _allowedContentTypes = configuration.GetSection($"{Settings.SectionName}:{Settings.AllowedContentTypes}").Get<List<string>>();

            BuildRules();
        }

        private void BuildRules()
        {
            RuleFor(x => x.Filename).NotEmpty().WithMessage("You must provide a filename");
            RuleFor(x => x.Bytes.Length).NotEqual(0).WithMessage("You must provide a file");
            RuleFor(x => x.Bytes.Length).Must(BeAValidFileSize).WithMessage("The file must be a valid size");
            RuleFor(x => x.Filename.GetContentType()).Must(BeAValidContentType).WithMessage("The file must be a valid content type");
        }

        private bool BeAValidFileSize(int bytes) => bytes < _maximumFileSizeInBytes;
        private bool BeAValidContentType(string contentType) => _allowedContentTypes.Contains(contentType);
    }
}
