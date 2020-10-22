using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using MediatR;

namespace DocumentManager.Core.Queries
{
    public class GetDocumentCollectionQuery : IRequest<ValueWrapper<IEnumerable<Document>>>
    {
        public string SortProperty { get; }
        public string SortDirection { get; }

        public GetDocumentCollectionQuery(string sortProperty, string sortDirection)
        {
            SortProperty = sortProperty;
            SortDirection = sortDirection;
        }
    }

    public class GetDocumentCollectionQueryHandler : IRequestHandler<GetDocumentCollectionQuery, ValueWrapper<IEnumerable<Document>>>
    {
        private readonly IDocumentRepository _repository;

        public GetDocumentCollectionQueryHandler(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValueWrapper<IEnumerable<Document>>> Handle(GetDocumentCollectionQuery request,
            CancellationToken cancellationToken)
        {
            var results = _repository.GetCollection(request.SortProperty,request.SortDirection, cancellationToken);

            return new ValueWrapper<IEnumerable<Document>>(results, true);
        }
    }
}
