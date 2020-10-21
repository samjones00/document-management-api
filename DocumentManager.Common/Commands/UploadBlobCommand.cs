using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;
using MediatR;

namespace DocumentManager.Core.Commands
{
    public class UploadBlobCommand : IRequest<ValueWrapper<bool>>
    {
        public byte[] Bytes { get; set; }
        public string Filename { get; set; }

        public UploadBlobCommand(string filename, byte[] bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class UploadBlobCommandHandler : IRequestHandler<UploadBlobCommand, ValueWrapper<bool>>
    {
        private readonly IStorageRepository _repository;

        public UploadBlobCommandHandler(IStorageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValueWrapper<bool>> Handle(UploadBlobCommand request, CancellationToken cancellationToken)
        {
            await _repository.Add(request.Filename, request.Bytes, cancellationToken);
            return new ValueWrapper<bool>(true);
        }
    }
}
