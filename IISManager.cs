using System;
using System.Linq;

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
            var sitesStr = _ps.ExecutePowerShell(command);
            Console.WriteLine("Выберите сайт:");
            var sites = sitesStr.Split().Skip(2).OrderBy(n => n).Select(n => n.Trim()).ToList();

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
