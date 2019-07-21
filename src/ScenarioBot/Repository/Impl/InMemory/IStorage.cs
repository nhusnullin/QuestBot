using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScenarioBot.Repository.Impl.InMemory
{
    public interface IStorage
    {
        Task<IDictionary<string, T>> ReadAsync<T>(
            string[] keys,
            CancellationToken cancellationToken = default (CancellationToken));

        Task WriteAsync(Dictionary<string, object> changes,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(string[] keys, CancellationToken cancellationToken = default (CancellationToken));
    }
}