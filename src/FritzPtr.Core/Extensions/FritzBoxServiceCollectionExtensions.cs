using System.Net;
using FritzPtr.Core;
using FritzPtr.Core.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FritzBoxServiceCollectionExtensions
{
    public static IServiceCollection AddFritzBoxClient(this IServiceCollection services)
    {
        services.AddHttpClient<IFritzBoxHostProvider, FritzBoxTr064Client>()
            .ConfigureHttpClient((provider, client) =>
            {
                client.BaseAddress = new("http://fritz.box:49000");
            })
            .ConfigurePrimaryHttpMessageHandler(provider => new HttpClientHandler()
            {
                Credentials = new NetworkCredential("User", "Password")
            });
        services.AddSingleton<IHostNameResolver, FritzBoxHostNameResolver>();

        return services;
    }
}