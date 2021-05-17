using System.Linq;
using System.Net;

namespace Function
{
    public static class DnsUtil
    {
        public static string ResolveDnsName(string dnsName)
            => string.Join(',', Dns.GetHostAddresses(dnsName).Select(a => a.ToString()));
    }
}
