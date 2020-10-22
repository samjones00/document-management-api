using System;
using Newtonsoft.Json;

namespace DocumentManager.Core.Models
{
    public class Document
    {
        public Document(string filename, long bytes, string contentType, DateTime dateCreated)
        {
            Id = Guid.NewGuid().ToString();
            Filename = filename;
            Bytes = bytes;
            ContentType = contentType;
            DateCreated = dateCreated;
        }

        [JsonProperty("id")]
        public string Id { get; set; } 

        public string Filename { get; }

        public DateTime DateCreated { get; }

        public long Bytes { get; }

        public string ContentType { get; }
    }
}
