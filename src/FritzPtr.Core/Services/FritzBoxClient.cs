using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using FritzPtr.Core.FritzBox;

namespace FritzPtr.Core.Services;

public sealed class FritzBoxTr064Client : IFritzBoxHostProvider
{
    private readonly HttpClient _http;

    public FritzBoxTr064Client(
        HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyCollection<FritzHost>> GetHostsAsync(
        CancellationToken ct = default)
    {
        // 1️⃣ SOAP: GetHostListPath
        var soapRequest = BuildGetHostListPathRequest();

        using var content = new StringContent(soapRequest, Encoding.UTF8);
        content.Headers.ContentType =
            new MediaTypeHeaderValue("text/xml");
        content.Headers.Add(
            "SOAPACTION",
            "\"urn:dslforum-org:service:Hosts:1#X_AVM-DE_GetHostListPath\"");

        using var response = await _http.PostAsync(
            "/upnp/control/hosts", content, ct);

        response.EnsureSuccessStatusCode();

        var soapResponse = await response.Content.ReadAsStringAsync(ct);

        var path = ParseHostListPath(soapResponse);

        // 2️⃣ Hostliste abrufen
        using var hostListResponse =
            await _http.GetAsync(path, ct);

        hostListResponse.EnsureSuccessStatusCode();

        var hostListXml =
            await hostListResponse.Content.ReadAsStringAsync(ct);

        return ParseHostList(hostListXml);
    }

    // -----------------------------
    // SOAP helpers
    // -----------------------------

    private static string BuildGetHostListPathRequest() =>
        """
        <?xml version="1.0" encoding="utf-8"?>
        <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"
                    s:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/">
          <s:Body>
            <u:getHostListPath
              xmlns:u="urn:dslforum-org:service:Hosts:1" />
          </s:Body>
        </s:Envelope>
        """;

    private static string ParseHostListPath(string soap)
    {
        var doc = XDocument.Parse(soap);

        return doc
            .Descendants()
            .First(x => x.Name.LocalName == "NewX_AVM-DE_HostListPath")
            .Value;
    }

    private static IReadOnlyCollection<FritzHost> ParseHostList(string xml)
    {
        var doc = XDocument.Parse(xml);

        return doc
            .Descendants("Item")
            .Select(x =>
            {
                var ip = IPAddress.TryParse(
                    (string?)x.Element("IPAddress"), out var parsedIp)
                    ? parsedIp
                    : null;

                if (ip == null)
                    return null;

                return new FritzHost(
                    ip,
                    (string?)x.Element("HostName") ?? string.Empty,
                    (string?)x.Element("MACAddress") ?? string.Empty,
                    (bool?)x.Element("Active") ?? false
                );
            })
            .Where(x => x != null)
            .ToList()!;
    }
}
