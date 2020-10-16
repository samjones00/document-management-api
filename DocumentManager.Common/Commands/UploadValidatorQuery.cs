using FluentValidation.Results;

using MediatR;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentManager.Common.Commands
{
    public class UploadValidatorQuery:IRequest<IEnumerable<ValidationResult>>
    {
        public string Filename { get; set; }
        public long Bytes { get; set; }

        public UploadValidatorQuery(string filename, long bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class UploadValidatorQueryHandler : IRequestHandler<UploadValidatorQuery, IEnumerable<ValidationResult>>
    {
        public Task<IEnumerable<ValidationResult>> Handle(UploadValidatorQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
