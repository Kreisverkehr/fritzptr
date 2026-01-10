namespace FritzPtr.Core.Dns;

public static class DnsMessageBuilder
{
    public static byte[] BuildRefused(byte[] query)
    {
        var response = (byte[])query.Clone();

        // QR = 1 (response)
        response[2] |= 0b1000_0000;

        // RCODE = 5 (REFUSED)
        response[3] &= 0b1111_0000;
        response[3] |= 0b0000_0101;

        // ANCOUNT = 0
        response[6] = response[7] = 0;

        return response;
    }
}
