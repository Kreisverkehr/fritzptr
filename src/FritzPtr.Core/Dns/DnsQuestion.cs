using FritzPtr.Core.Extensions;

namespace FritzPtr.Core.Dns;

public sealed class DnsQuestion
{
    public string Name { get; }
    public ushort Type { get; }
    public ushort Class { get; }

    public DnsQuestion(ReadOnlySpan<byte> data, ref int offset)
    {
        Name = data.ReadQName(ref offset);
        Type = data.ReadU16(offset); 
        offset += 2;
        Class = data.ReadU16(offset); 
        offset += 2;
    }
}
