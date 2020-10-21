using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using MediatR;

namespace DocumentManager.Core.Commands
{
    public class DeleteBlobCommand : IRequest<ValueWrapper<bool>>
    {
        public string Filename { get; set; }

        public DeleteBlobCommand(string filename)
        {
            Filename = filename;
        }
    }

    public class DeleteBlobCommandHandler : IRequestHandler<DeleteBlobCommand, ValueWrapper<bool>>
    {
        private readonly IStorageRepository _repository;

        public DeleteBlobCommandHandler(IStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValueWrapper<bool>> Handle(DeleteBlobCommand request, CancellationToken cancellationToken)
        {
            await _repository.Delete(request.Filename, cancellationToken);
            return new ValueWrapper<bool>(true);
        }
    }
}
