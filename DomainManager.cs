using System;

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
                Console.Write("Введите ip адрес сервера:");
                var ip = Console.ReadLine();
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
