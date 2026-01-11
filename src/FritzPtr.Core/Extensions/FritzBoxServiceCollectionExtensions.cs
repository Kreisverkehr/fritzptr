using System.Net;
using FritzPtr.Core;
using FritzPtr.Core.FritzBox;
using FritzPtr.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class FritzBoxServiceCollectionExtensions
{
    public static IServiceCollection AddFritzBoxClient(this IServiceCollection services) => services
        .AddOptions<FritzBoxClientOptions>()
            .Configure<IConfiguration>((o, c) => c.GetSection(FritzBoxClientOptions.SECTION_NAME).Bind(o)).Services
        .AddHttpClient<IFritzBoxHostProvider, FritzBoxTr064Client>()
            .ConfigureHttpClient((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<FritzBoxClientOptions>>().Value;
                client.BaseAddress = new($"http://{options.Host}:49000");
            })
            .ConfigurePrimaryHttpMessageHandler(provider => 
            {
                var options = provider.GetRequiredService<IOptions<FritzBoxClientOptions>>().Value;
                return new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(options.Username, options.Password)
                };
            }).Services
        .AddSingleton<IHostNameResolver, FritzBoxHostNameResolver>()
        ;
}