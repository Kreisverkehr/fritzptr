using System.Net;

namespace FritzPtr.Core.FritzBox;

public sealed record FritzHost(
    IPAddress IpAddress,
    string HostName,
    string MacAddress,
    bool Active
);
