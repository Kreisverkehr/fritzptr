using FritzPtr.Core.Extensions;

namespace FritzPtr.Core.Dns;

public readonly struct DnsHeader
{
    public ushort Id { get; }
    public ushort Flags { get; }
    public ushort QuestionCount { get; }

    public DnsHeader(ReadOnlySpan<byte> data)
    {
        Id = data.ReadU16(0);
        Flags = data.ReadU16(2);
        QuestionCount = data.ReadU16(4);
    }
}
