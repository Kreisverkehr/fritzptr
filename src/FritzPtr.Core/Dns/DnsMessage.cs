namespace FritzPtr.Core.Dns;

public sealed class DnsMessage
{
    public DnsHeader Header { get; }
    public DnsQuestion Question { get; }

    public DnsMessage(ReadOnlySpan<byte> message)
    {
        int offset = 0;

        Header = new DnsHeader(message);
        offset = 12;

        if (Header.QuestionCount == 0)
            throw new InvalidOperationException("No DNS question present");

        Question = new DnsQuestion(message, ref offset);
    }
}
