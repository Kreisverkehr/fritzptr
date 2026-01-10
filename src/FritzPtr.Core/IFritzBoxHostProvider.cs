using System.Net;
using FritzPtr.Core.FritzBox;

namespace FritzPtr.Core;

public interface IFritzBoxHostProvider
{
    Task<IReadOnlyCollection<FritzHost>> GetHostsAsync(CancellationToken cancellationToken = default);
}
