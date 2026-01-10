using System.Net;
using FritzPtr.Core.Dns;

namespace FritzPtr.Core.Extensions;

public static class DnsMEssageExtensions
{
    const ushort PTR = 12;

    extension(DnsMessage msg)
    {
        public bool IsPtrQuery 
            => msg.Question.Type == PTR &&
                (
                    msg.Question.Name.EndsWith(".in-addr.arpa", StringComparison.OrdinalIgnoreCase) ||
                    msg.Question.Name.EndsWith(".ip6.arpa", StringComparison.OrdinalIgnoreCase)
                );
                
        public IPAddress ParseIpFromPtr()
        {
            if(msg.Question.Name.EndsWith(".ip6.arpa", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException("IPv6 is not supported.");
            
            var parts = msg.Question.Name
                .Replace(".in-addr.arpa", "", StringComparison.OrdinalIgnoreCase)
                .Split('.')
                .Reverse()
                .ToArray();

            return IPAddress.Parse(string.Join('.', parts));
        }
    }
}