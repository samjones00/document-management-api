using DocumentManager.Common.Models;

namespace DocumentManager.Common.Interfaces
{
    public interface IUploadItemFactory
    {
        Document Create(string filename, long length);
    }
}