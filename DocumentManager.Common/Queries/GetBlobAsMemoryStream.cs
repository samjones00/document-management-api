using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;
using MediatR;
using DocumentManager.Core.Interfaces;

namespace DocumentManager.Core.Queries
{
    public class GetBlobAsMemoryStream : IRequest<ValueWrapper<MemoryStream>>
    {
        public string Filename { get; }

        public GetBlobAsMemoryStream(string filename)
        {
            Filename = filename;
        }
    }

    public class GetBlobAsMemoryStreamHandler : IRequestHandler<GetBlobAsMemoryStream, ValueWrapper<MemoryStream>>
    {
        private readonly IStorageRepository _repository;

        public GetBlobAsMemoryStreamHandler(IStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValueWrapper<MemoryStream>> Handle(GetBlobAsMemoryStream request, CancellationToken cancellationToken)
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
