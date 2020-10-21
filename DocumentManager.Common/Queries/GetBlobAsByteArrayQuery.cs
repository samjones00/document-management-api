using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using DocumentManager.Core.Interfaces;

namespace DocumentManager.Core.Queries
{
    public class GetBlobAsByteArrayQuery : IRequest<ValueWrapper<MemoryStream>>
    {
        public string Filename { get; set; }

        public GetBlobAsByteArrayQuery(string filename)
        {
            Filename = filename;
        }
    }

    public class GetBlobAsByteArrayQueryHandler : IRequestHandler<GetBlobAsByteArrayQuery, ValueWrapper<MemoryStream>>
    {
        private readonly IStorageRepository _repository;
        private readonly ILogger<GetBlobAsByteArrayQueryHandler> _logger;

        public GetBlobAsByteArrayQueryHandler(IStorageRepository repository,
            ILogger<GetBlobAsByteArrayQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ValueWrapper<MemoryStream>> Handle(GetBlobAsByteArrayQuery request,
            CancellationToken cancellationToken)
        {
            return new ValueWrapper<MemoryStream>(await _repository.Get(request.Filename, cancellationToken), true);
        }
    }
}
