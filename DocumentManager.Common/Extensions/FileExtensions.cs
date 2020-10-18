using Microsoft.AspNetCore.StaticFiles;

namespace DocumentManager.Core.Extensions
{
    public static class FileExtensions
    {
        public static string GetContentType(this string filename)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filename, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
