using DocumentManager.Common.Extensions;
using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Models;

namespace DocumentManager.Common
{
    public class UploadItemFactory : IUploadItemFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public UploadItemFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public Document Create(string filename, long bytes)
        {
            return new Document(filename, bytes, filename.GetContentType(), _dateTimeProvider.UtcNow());
        }
    }
}