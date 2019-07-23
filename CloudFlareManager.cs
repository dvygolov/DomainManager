using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DomainManager
{
    public class CloudFlareManager
    {
        private readonly string _domainsFilePath;
        private readonly CmdExecutor _cmd;

        public CloudFlareManager(string domainsFilePath)
        {
            _domainsFilePath = domainsFilePath;
            _cmd = new CmdExecutor(Path.GetDirectoryName(domainsFilePath));
        }

        public void AddDomains(string domains, string ip)
        {
            WriteDomainsToFile(domains);
            var command = $"php build/cloud.php add-domains --enable-proxy --ip \"{ip}\"";
            _cmd.ExecuteCmd(command);
        }

        public void CancelDomains(string domains)
        {
            WriteDomainsToFile(domains);
            var command = $"php build/cloud.php remove-domains";
            _cmd.ExecuteCmd(command);
        }

        private void WriteDomainsToFile(string domains)
        {
            File.WriteAllLines(_domainsFilePath, domains.Split(','));
        }

        public string GetNameServers(string domains)
        {
            var domainsHashSet=domains.Split(',').ToHashSet();
            var extractNSRegex = @"^\s{2}(?<domain>\w+\.\w{2,3})\s+.{32}\s+(?<ns>.*?)\s{3}active\s+$";
            var command = $"php build/cloud.php show-domains";
            var res = _cmd.ExecuteCmd(command);
            var matches = Regex.Matches(res, extractNSRegex);
            foreach(Match m in matches)
            {
                if (domainsHashSet.Contains(m.Groups["domain"].Value))
                    return m.Groups["ns"].Value;
            }
            throw new Exception("Не найдены NS-сервера для доменов "+domains);
        }
    }
}
