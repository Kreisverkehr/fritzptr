using System.Net;

namespace FritzPtr.Core;

public interface IHostNameResolver
{
    Task<string?> ResolveAsync(
        IPAddress ip,
        CancellationToken cancellationToken = default);
}
