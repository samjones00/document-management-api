using DocumentManager.Core.Extensions;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Models;

namespace DocumentManager.Core.Factories
{
    public class DocumentFactory : IUploadItemFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public DocumentFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public Document Create(string filename, long bytes)
        {
            return new Document(filename, bytes, filename.GetContentType(), _dateTimeProvider.UtcNow());
        }
    }
}