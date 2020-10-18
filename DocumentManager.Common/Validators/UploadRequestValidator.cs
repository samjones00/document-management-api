using DocumentManager.Core.Extensions;
using DocumentManager.Core.Models;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace DocumentManager.Core.Validators
{
    public class UploadRequestValidator : AbstractValidator<UploadRequest>
    {
        private readonly ValidatorSettings _settings;

        public UploadRequestValidator(IConfiguration configuration)
        {
            _settings = configuration.GetSection(ValidatorSettings.SectionName).Get<ValidatorSettings>();
            BuildRules();
        }

        private void BuildRules()
        {
            RuleFor(x => x.Filename).NotEmpty().WithMessage("You must provide a filename");
            RuleFor(x => x.Bytes.Length).NotEqual(0).WithMessage("You most supply a file");
            RuleFor(x => x.Bytes.Length).Must(BeAValidFileSize).WithMessage("The file must be a valid size");
            RuleFor(x => x.Filename.GetContentType()).Must(BeAValidContentType).WithMessage("You must provide a valid content type");
        }

        private bool BeAValidFileSize(int bytes) => bytes < _settings.MaximumFileSizeInBytes;
        private bool BeAValidContentType(string contentType) => _settings.AllowedContentTypes.Contains(contentType);
    }
}
