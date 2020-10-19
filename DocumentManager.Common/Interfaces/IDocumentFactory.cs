using DocumentManager.Core.Models;

namespace DocumentManager.Core.Interfaces
{
    public interface IDocumentFactory
    {
        ValueWrapper<Document> Create(string filename, long length);
    }
}