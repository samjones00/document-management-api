using DocumentManager.Core.Extensions;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;

namespace DocumentManager.Core.Factories
{
    public class DocumentFactory : IDocumentFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public DocumentFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public ValueWrapper<Document> Create(string filename, long bytes)
        {
            if (string.IsNullOrEmpty(filename) || bytes == 0)
            {
                return new ValueWrapper<Document>(null,false);
            }

            return new ValueWrapper<Document>(new Document(filename, bytes, filename.GetContentType(), _dateTimeProvider.UtcNow),true);
        }
    }
}