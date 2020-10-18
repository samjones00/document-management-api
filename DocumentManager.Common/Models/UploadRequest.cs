namespace DocumentManager.Core.Models
{
    public struct UploadRequest
    {
        public string Filename { get; set; }
        public string Data { get; set; }
        public byte[] Bytes { get; set; }
    }
}
