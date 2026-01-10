using System.Collections.Concurrent;
using System.Net;

namespace FritzPtr.Core.Services;

internal sealed record CachedHostEntry(
    IPAddress Ip,
    string HostName,
    DateTime ExpiresAt
);

public sealed class FritzBoxHostNameResolver : IHostNameResolver
{
    private readonly IFritzBoxHostProvider _provider;
    private readonly TimeSpan _ttl = TimeSpan.FromSeconds(60);
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private volatile Dictionary<IPAddress, CachedHostEntry> _cache =
        new();

    public FritzBoxHostNameResolver(IFritzBoxHostProvider provider)
    {
        _provider = provider;
    }

    public async Task<string?> ResolveAsync(
        IPAddress ip,
        CancellationToken ct = default)
    {
        // 1️⃣ Cache Hit?
        if (_cache.TryGetValue(ip, out var entry) &&
            entry.ExpiresAt > DateTime.UtcNow)
        {
            return entry.HostName;
        }

        // 2️⃣ Refresh Cache (nur ein Thread!)
        await _refreshLock.WaitAsync(ct);
        try
        {
            // Double-check
            if (_cache.TryGetValue(ip, out entry) &&
                entry.ExpiresAt > DateTime.UtcNow)
            {
                return entry.HostName;
            }

            await RefreshCacheAsync(ct);
        }
        finally
        {
            _refreshLock.Release();
        }

        return _cache.TryGetValue(ip, out entry)
            ? entry.HostName
            : null;
    }

    // -----------------------------
    // Cache Refresh
    // -----------------------------

    private async Task RefreshCacheAsync(CancellationToken ct)
    {
        var hosts = await _provider.GetHostsAsync(ct);
        var expires = DateTime.UtcNow.Add(_ttl);

        Dictionary<IPAddress, CachedHostEntry> cache = [];
        foreach(var host in hosts.Where(h => !string.IsNullOrWhiteSpace(h.HostName)))
        {
            cache.TryAdd(host.IpAddress, new(
                    host.IpAddress,
                    Sanitize(host.HostName),
                    expires));
        }
        _cache = cache;
    }

    private static string Sanitize(string name)
    {
        return name
            .Trim()
            .ToLowerInvariant()
            .Replace(' ', '-');
    }
}
