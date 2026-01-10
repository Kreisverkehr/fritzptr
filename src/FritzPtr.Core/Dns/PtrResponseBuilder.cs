using System.Net;
using System.Text;

namespace FritzPtr.Core.Dns;

public static class PtrResponseBuilder
{
    public static byte[] Build(
        byte[] query,
        string queryName,
        string ptrTarget,
        uint ttlSeconds = 60)
    {
        using var ms = new MemoryStream();

        // -----------------------------
        // Header (12 Bytes)
        // -----------------------------

        // ID (copy from query)
        ms.Write(query, 0, 2);

        // Flags
        // QR=1, AA=1, RD=copy, RCODE=0
        ushort flags = (ushort)(
            0b1000_0100_0000_0000 |   // QR + AA
            ((query[2] & 0b0000_0001) << 8) // RD
        );

        WriteU16(ms, flags);

        // QDCOUNT = 1
        WriteU16(ms, 1);

        // ANCOUNT = 1
        WriteU16(ms, 1);

        // NSCOUNT, ARCOUNT = 0
        WriteU16(ms, 0);
        WriteU16(ms, 0);

        // -----------------------------
        // Question (kopiert)
        // -----------------------------
        int questionLength = GetQuestionLength(query);
        ms.Write(query, 12, questionLength);

        // -----------------------------
        // Answer
        // -----------------------------

        // NAME (QNAME erneut, keine Compression)
        WriteQName(ms, queryName);

        // TYPE = PTR (12)
        WriteU16(ms, 12);

        // CLASS = IN (1)
        WriteU16(ms, 1);

        // TTL
        WriteU32(ms, ttlSeconds);

        // RDATA vorbereiten
        using var rdata = new MemoryStream();
        WriteQName(rdata, ptrTarget);

        // RDLENGTH
        WriteU16(ms, (ushort)rdata.Length);

        // RDATA
        rdata.Position = 0;
        rdata.CopyTo(ms);

        return ms.ToArray();
    }

    // -----------------------------
    // Helpers
    // -----------------------------

    private static int GetQuestionLength(byte[] query)
    {
        int offset = 12;

        while (query[offset] != 0)
            offset += query[offset] + 1;

        return (offset + 1 + 4) - 12; // null + QTYPE + QCLASS
    }

    private static void WriteQName(Stream s, string name)
    {
        foreach (var label in name.Split('.'))
        {
            s.WriteByte((byte)label.Length);
            s.Write(Encoding.ASCII.GetBytes(label));
        }
        s.WriteByte(0x00);
    }

    private static void WriteU16(Stream s, ushort value)
    {
        s.WriteByte((byte)(value >> 8));
        s.WriteByte((byte)(value & 0xFF));
    }

    private static void WriteU32(Stream s, uint value)
    {
        s.WriteByte((byte)(value >> 24));
        s.WriteByte((byte)((value >> 16) & 0xFF));
        s.WriteByte((byte)((value >> 8) & 0xFF));
        s.WriteByte((byte)(value & 0xFF));
    }
}
