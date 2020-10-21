using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using MediatR;

namespace DocumentManager.Core.Commands
{
    public class CreateDocumentCommand : IRequest<ValueWrapper<bool>>
    {
        public string Filename { get; set; }
        public long Bytes { get; set; }

        public CreateDocumentCommand(string filename, long bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, ValueWrapper<bool>>
    {
        private readonly IDocumentRepository _repository;
        private readonly IDocumentFactory _documentFactory;

        public CreateDocumentCommandHandler(IDocumentRepository repository, IDocumentFactory documentFactory)
        {
            _repository = repository;
            _documentFactory = documentFactory;
        }

        public async Task<ValueWrapper<bool>> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {

            var document = _documentFactory.Create(request.Filename, request.Bytes);

            if (!document.IsSuccessful)
            {
                return new ValueWrapper<bool>(false);
            }

            await _repository.Add(document.Value, cancellationToken);

            return new ValueWrapper<bool>(true);
        }
    }
}
