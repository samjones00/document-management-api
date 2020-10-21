using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using MediatR;

namespace DocumentManager.Core.Queries
{
    public class GetDocumentQuery : IRequest<ValueWrapper<Document>>
    {
        public string Filename { get; }

        public GetDocumentQuery(string filename)
        {
            Filename = filename;
        }
    }

    public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, ValueWrapper<Document>>
    {
        private readonly IDocumentRepository _repository;

        public GetDocumentQueryHandler(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValueWrapper<Document>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            var document = await _repository.Get(request.Filename);

            if (document == null)
            {
                return new ValueWrapper<Document>(null, false);
            }

            return new ValueWrapper<Document>(document, true);
        }
    }
}
