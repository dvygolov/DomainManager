using System;
using System.Linq;
using System.Net.Sockets;

namespace DomainManager
{
    public class DomainManager
    {
        private readonly FreenomManager _fnm;
        private readonly CloudFlareManager _cfm;
        private readonly IISManager _im;

        public DomainManager(FreenomManager fnm, CloudFlareManager cfm, IISManager im)
        {
            _fnm = fnm;
            _cfm = cfm;
            _im = im;
        }

        public void CancelDomain(string domains, bool useCloudFlare = true)
        {
            _im.CancelDomains(domains);
            if (useCloudFlare)
                _cfm.CancelDomains(domains);
            _fnm.CancelDomains(domains);
        }

        public void AddDomain(string domains, string siteName, bool useCloudFlare = true)
        {
            if (useCloudFlare)
            {
                string hostName = System.Net.Dns.GetHostName();
                var defaultIp=System.Net.Dns.GetHostEntry(hostName).AddressList.
                    First(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();
                Console.Write($"Введите ip адрес сервера (по умолчанию {defaultIp}):");
                var ip = Console.ReadLine();
                if (string.IsNullOrEmpty(ip))
                    ip=defaultIp;
                _cfm.AddDomains(domains, ip);
                var nameServers = _cfm.GetNameServers(domains);
                _fnm.ModifyNameServers(domains, nameServers);
            }
            else
            {
                //TODO:сделать тут создание SSL-сертификата
            }
            _im.AddDomains(domains, siteName);
        }
    }
}
