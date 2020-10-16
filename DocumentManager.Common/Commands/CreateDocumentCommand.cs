using DocumentManager.Common.Interfaces;
using MediatR;
using Microsoft.Azure.Cosmos;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentManager.Common.Commands
{
    public class CreateDocumentCommand : IRequest<bool>
    {
        public string Filename { get; set; }
        public long Bytes { get; set; }

        public CreateDocumentCommand(string filename, long bytes)
        {
            Filename = filename;
            Bytes = bytes;
        }
    }

    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, bool>
    {
        private readonly IUploadItemFactory _uploadItemFactory;
        private readonly CosmosClient _cosmosClient;

        public CreateDocumentCommandHandler(IResolver<CosmosClient> resolver,IUploadItemFactory uploadItemFactory)
        {
            _uploadItemFactory = uploadItemFactory;
            _cosmosClient = resolver.Resolve();
        }

        public async Task<bool> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            var uploadItem = _uploadItemFactory.Create(request.Filename,request.Bytes);

            var container = _cosmosClient.GetContainer(Constants.Cosmos.DatabaseName, Constants.Cosmos.ContainerName);
            await container.CreateItemAsync(uploadItem);

            return true;
        }
    }
}
