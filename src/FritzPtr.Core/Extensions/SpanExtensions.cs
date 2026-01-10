using System.Text;

namespace FritzPtr.Core.Extensions;

internal static class SpanExtensions
{
    internal static ushort ReadU16(this ReadOnlySpan<byte> data, int offset)
        => (ushort)((data[offset] << 8) | data[offset + 1]);

    internal static string ReadQName(this ReadOnlySpan<byte> data, ref int offset)
    {
        var labels = new List<string>();

        while (true)
        {
            byte length = data[offset++];

            if (length == 0)
                break;

            // ❗ No Compression Support (0b11xxxxxx)
            if ((length & 0b1100_0000) != 0)
                throw new NotSupportedException("Name compression not supported");

            labels.Add(
                Encoding.ASCII.GetString(
                    data.Slice(offset, length)));

            offset += length;
        }

        return string.Join('.', labels);
}

}