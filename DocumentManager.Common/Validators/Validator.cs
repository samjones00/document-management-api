using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.Common.Models;

namespace DocumentManager.Common.Validators
{
    public class Validator : AbstractValidator<UploadItem>
    {
        public Validator()
        {
            RuleFor(x => x.Filename).NotEmpty().WithMessage("Yu must provide a filename");
            RuleFor(x => x.Bytes).NotEqual(0).WithMessage("You most supply a file");
            RuleFor(x => x.Bytes).Must(BeAValidFileSize).WithMessage("The file must be an allowed size");
            RuleFor(x => x.ContentType).Must(BeAValidContentType).WithMessage("You must provide an allowed content type");
        }

        private bool BeAValidFileSize(long bytes)
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
