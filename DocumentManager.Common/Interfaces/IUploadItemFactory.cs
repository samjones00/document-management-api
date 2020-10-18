using DocumentManager.Core.Models;

namespace DocumentManager.Core.Interfaces
{
    public interface IUploadItemFactory
    {
        Document Create(string filename, long length);
    }
}