using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;

namespace DocumentManager.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<bool> Delete(string filename, CancellationToken cancellationToken);
        Task Add(Document document, CancellationToken cancellationToken);
        Document GetSingle(string filename);
        IEnumerable<Document> GetCollection(string sortProperty, string sortDirection, CancellationToken cancellationToken);
    }
}
