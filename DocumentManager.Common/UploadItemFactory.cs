using DocumentManager.Common.Interfaces;
using DocumentManager.Common.Models;
using DocumentManager.Common.Providers;
using Microsoft.AspNetCore.StaticFiles;

namespace DocumentManager.Common
{
    public class UploadItemFactory : IUploadItemFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public UploadItemFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public UploadItem Create(string filename, long bytes)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filename, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return new UploadItem(filename, bytes, contentType, _dateTimeProvider.UtcNow());
        }
    }
}