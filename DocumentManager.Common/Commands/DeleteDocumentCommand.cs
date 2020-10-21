using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using MediatR;

namespace DocumentManager.Core.Commands
{
    public class DeleteDocumentCommand : IRequest<ValueWrapper<bool>>
    {
        public string Filename { get; set; }

        public DeleteDocumentCommand(string filename)
        {
            Filename = filename;
        }
    }

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, ValueWrapper<bool>>
    {
        private readonly IDocumentRepository _repository;

        public DeleteDocumentCommandHandler(IDocumentRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValueWrapper<bool>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            await _repository.Delete(request.Filename, cancellationToken);
            return new ValueWrapper<bool>(true);
        }
    }
}
