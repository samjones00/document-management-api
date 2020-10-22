using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using DocumentManager.Core.Interfaces;

namespace DocumentManager.Core.Queries
{
    public class GetBlobAsMemoryStreamQuery : IRequest<ValueWrapper<MemoryStream>>
    {
        public string Filename { get; }

        public GetBlobAsMemoryStreamQuery(string filename)
        {
            Filename = filename;
        }
    }

    public class GetBlobAsMemoryStreamQueryHandler : IRequestHandler<GetBlobAsMemoryStreamQuery, ValueWrapper<MemoryStream>>
    {
        private readonly IStorageRepository _repository;

        public GetBlobAsMemoryStreamQueryHandler(IStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValueWrapper<MemoryStream>> Handle(GetBlobAsMemoryStreamQuery request, CancellationToken cancellationToken)
        {
            var response = await _repository.Get(request.Filename, cancellationToken);

            if(response == null || response.Length == 0)
            {
                return new ValueWrapper<MemoryStream>(null, false);
            }

            return new ValueWrapper<MemoryStream>(response, true);
        }
    }
}
