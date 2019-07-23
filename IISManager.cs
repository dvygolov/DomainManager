using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DomainManager
{
    public class IISManager
    {
        private CmdExecutor _ps;

        public IISManager()
        {
            _ps = new CmdExecutor(string.Empty);
        }

        public string GetWebsiteName()
        {
            var command = "Get-Website | Select-Object -Property Name";
            var output = _ps.ExecutePowerShell(command);
            var sitesRegex=@"----[\s\r\n]+(?<sites>.*)[\r\n]{4}";
            var m=Regex.Match(output,sitesRegex,RegexOptions.Singleline);
            var sitesStr=m.Groups["sites"].Value;

            Console.WriteLine("Выберите сайт:");
            var sites = sitesStr.Split(new char[] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries)
                .OrderBy(n => n).Select(n => n.Trim()).ToList();

            int i = 1;
            foreach (var s in sites)
            {
                Console.WriteLine($"{i}.{s}");
                i++;
            }
            Console.Write("Ваш выбор:");
            var indx = int.Parse(Console.ReadLine());
            return sites[indx - 1];
        }

        public void CancelDomains(string domains)
        {
            foreach (var d in domains.Split(','))
            {
                Console.WriteLine($"Удаляем из IIS домен {d}...");
                _ps.ExecutePowerShell($"Remove-WebBinding -BindingInformation \"*:80:{d}\"");
                _ps.ExecutePowerShell($"Remove-WebBinding -BindingInformation \"*:443:{d}\"");
                Console.WriteLine($"Домен {d} успешно удалён!");
            }
        }

        public void AddDomains(string domains, string siteName)
        {
            foreach (var d in domains.Split(','))
            {
                Console.WriteLine($"Добавляем в IIS домен {d}...");
                _ps.ExecutePowerShell($"New-WebBinding -BindingInformation \"*:80:{d}\"");
                _ps.ExecutePowerShell($"New-WebBinding -BindingInformation \"*:443:{d}\"");
                Console.WriteLine($"Домен {d} добавлен!");
            }
        }
    }
}
