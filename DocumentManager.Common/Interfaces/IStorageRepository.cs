using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentManager.Core.Interfaces
{
    public interface IStorageRepository
    {
        Task Delete(string filename, CancellationToken cancellationToken);
        Task Add(string filename, byte[] bytes, CancellationToken cancellationToken);
        Task<MemoryStream> Get(string filename, CancellationToken cancellationToken);
    }
}
