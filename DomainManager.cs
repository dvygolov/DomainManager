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

        public void CreateNewSite()
        {
            Console.Write("Введите имя нового сайта:");
            var siteName = Console.ReadLine();
            Console.Write("Введите путь к zip-архиву c файлами сайта:");
            var zipPackagePath = Console.ReadLine();
            _im.CreateNewSite(siteName, zipPackagePath);
            AddDomain(siteName);
        }

        public void CancelDomain(string domains = "")
        {
            if (string.IsNullOrEmpty(domains))
                domains = _fnm.SelectDomains();
            _im.CancelDomains(domains);
            _cfm.CancelDomains(domains);
            _fnm.CancelDomains(domains);
        }

        public void AddDomain(string siteName = "")
        {
            if (string.IsNullOrEmpty(siteName))
                siteName = _im.GetWebsiteName();
            var domains = _fnm.SelectDomains();
            var ip = GetServerIpAddress();
            _cfm.AddDomains(domains, ip);
            var nameServers = _cfm.GetNameServers(domains);
            _fnm.ModifyNameServers(domains, nameServers);
            _im.AddDomains(domains, siteName);
        }

        private static string GetServerIpAddress()
        {
            string hostName = System.Net.Dns.GetHostName();
            var defaultIp = System.Net.Dns.GetHostEntry(hostName).AddressList.
                First(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();
            Console.Write($"Введите ip адрес сервера (по умолчанию {defaultIp}):");
            var ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip))
                ip = defaultIp;
            return ip;
        }

        internal void DeleteSite()
        {
            var siteName = _im.GetWebsiteName();
            var domains = _im.GetWebsiteDomains(siteName);
            _im.DeleteWebsite(siteName);
            Console.WriteLine($"У данного сайта были следующие домены:{domains}.");
            Console.Write("Удалить эти домены?(Y/N)");
            if (YesNoSelector.ReadAnswerEqualsYes())
            {
                _im.CancelDomains(domains);
            }
        }
    }
}
