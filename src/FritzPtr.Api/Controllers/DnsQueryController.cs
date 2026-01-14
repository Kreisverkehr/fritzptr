using FritzPtr.Core.Extensions;
using FritzPtr.Core.Dns;
using Microsoft.AspNetCore.Mvc;
using FritzPtr.Core;

namespace FritzPtr.Api.Controllers;

[ApiController]
public class DnsQueryController : ControllerBase
{
    private const string DnsMimeType = "application/dns-message";
    private readonly IHostNameResolver _hostNameResolver;

    public DnsQueryController(IHostNameResolver hostNameResolver)
    {
        _hostNameResolver = hostNameResolver;
    }

    [HttpGet("/dns-query")]
    public async Task<IActionResult> Get([FromQuery] string dns)
    {
        if (string.IsNullOrWhiteSpace(dns))
            return BadRequest("Missing dns query parameter");

        byte[] query;
        try
        {
            query = Convert.FromBase64String(
                dns.Replace('-', '+').Replace('_', '/')
                   .PadRight(dns.Length + (4 - dns.Length % 4) % 4, '='));
        }
        catch
        {
            return BadRequest("Invalid base64url encoding");
        }

        return await HandleDnsQuery(query);
    }

    [HttpPost("/dns-query")]
    public async Task<IActionResult> Post()
    {
        if (Request.ContentType != DnsMimeType)
            return StatusCode(415, "Unsupported Media Type");

        using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);

        return await HandleDnsQuery(ms.ToArray());
    }

    private async Task<IActionResult> HandleDnsQuery(byte[] dnsQuery)
    {
        DnsMessage msg = new(dnsQuery);
        byte[] response;
        response = DnsMessageBuilder.BuildRefused(dnsQuery);

        if (msg.IsPtrQuery)
        {
            var ip = msg.ParseIpFromPtr();
            var hostname = await _hostNameResolver.ResolveAsync(ip);

            if (hostname != null)
                response = PtrResponseBuilder.Build(
                    dnsQuery,
                    msg.Question.Name,
                    hostname.EndsWith("fritz.box") ? hostname : $"{hostname}.fritz.box"
                );
        }

        return File(response, DnsMimeType);
    }
}
