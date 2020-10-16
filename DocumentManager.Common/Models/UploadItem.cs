using System;

namespace DocumentManager.Common.Models
{
    public class UploadItem
    {
        public UploadItem(string filename, long bytes, string contentType, DateTime dateCreated)
        {
            Filename = filename;
            Bytes = bytes;
            ContentType = contentType;
            DateCreated = dateCreated;
        }

        public string id { get; } = Guid.NewGuid().ToString();
        public string Filename { get; }
        public DateTime DateCreated { get; }
        public long Bytes { get; }
        public string ContentType { get; }
    }
}
