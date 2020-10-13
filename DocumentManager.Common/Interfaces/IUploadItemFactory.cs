using DocumentManager.Common.Models;

namespace DocumentManager.Common.Interfaces
{
    public interface IUploadItemFactory
    {
        UploadItem Create(string filename, long length);
    }
}