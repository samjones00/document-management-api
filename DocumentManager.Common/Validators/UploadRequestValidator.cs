using FluentValidation;
using System;
using System.Linq;
using DocumentManager.Common.Models;
using DocumentManager.Common.Extensions;

namespace DocumentManager.Common.Validators
{
    public class UploadRequestValidator : AbstractValidator<UploadRequest>
    {
        public UploadRequestValidator()
        {
            RuleFor(x => x.Filename).NotEmpty().WithMessage("Yu must provide a filename");
            RuleFor(x => x.Bytes.Length).NotEqual(0).WithMessage("You most supply a file");
            RuleFor(x => x.Bytes.Length).Must(BeAValidFileSize).WithMessage("The file must be an allowed size");
            RuleFor(x => x.Filename.GetContentType()).Must(BeAValidContentType).WithMessage("You must provide an allowed content type");
        }

        private bool BeAValidFileSize(int bytes)
        {
            var maxSize = 5000;

            return bytes < maxSize;
        }

        private bool BeAValidContentType(string contentType)
        {
            var allowed = new[]
            {
                "application/pdf"
            };

            return allowed.Contains(contentType);
        }
    }
}
