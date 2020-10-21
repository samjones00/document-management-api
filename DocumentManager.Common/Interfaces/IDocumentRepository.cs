using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.Core.Models;

namespace DocumentManager.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> Get(Expression<Func<Document, bool>> Query, string sortProperty, CancellationToken cancellationToken);
        Task Delete(string filename, CancellationToken cancellationToken);
        Task Add(Document document, CancellationToken cancellationToken);
    }
}
