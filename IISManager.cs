using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DomainManager
{
    public class IISManager
    {
        private readonly CmdExecutor _ps;
        private readonly string _sitesPath;
        private readonly string _archivePath;

        public IISManager(string sitesPath, string archivePath = "")
        {
            _ps = new CmdExecutor(string.Empty);
            _sitesPath = sitesPath;
            _archivePath = archivePath;
        }

        public string GetWebsiteName()
        {
            var command = "Get-Website | Select-Object -Property Name";
            var output = _ps.ExecutePowerShell(command);
            var sitesRegex = @"----[\s\r\n]+(?<sites>.*)[\r\n]{4}";
            var m = Regex.Match(output, sitesRegex, RegexOptions.Singleline);
            var sitesStr = m.Groups["sites"].Value;

            Console.WriteLine("Выберите сайт:");
            var sites = sitesStr.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
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

        internal void StartSite(string siteName)
        {
            Console.WriteLine($"Запускаем сайт {siteName}...");
            var command = $"Start-Website -Name {siteName}";
            var output = _ps.ExecutePowerShell(command);
            Console.WriteLine("Сайт запущен.");
        }

        public void RemoveNewWebsiteBinding(string siteName)
        {
            var command = $"Remove-WebBinding -Name {siteName} -Port 80 -HostHeader NewSite";
            var output = _ps.ExecutePowerShell(command);
        }


        public string GetWebsiteDomains(string siteName)
        {
            var command = $"Get-WebBinding -Name {siteName}";
            var output = _ps.ExecutePowerShell(command);
            var sitesRegex = @":\d+:(?<domain>[^\s]+)";
            var matches = Regex.Matches(output, sitesRegex, RegexOptions.Singleline);
            var domains=string.Join(',',matches.Select(m=>m.Groups["domain"].Value));
            return domains;
        }

        internal void CreateNewSite(string siteName, string zipPackagePath)
        {
            Console.WriteLine($"Создаём в IIS сайт {siteName}...");
            var siteFolder = Path.Combine(_sitesPath, siteName);
            ZipHelper.ExtractZipFile(zipPackagePath, string.Empty, siteFolder);
            _ps.ExecutePowerShell($"New-WebSite -Name {siteName} -Port 80 -HostHeader NewSite -PhysicalPath {siteFolder}");
            Console.WriteLine($"Сайт создан!");
        }

        internal void DeleteWebsite(string siteName)
        {
            Console.WriteLine($"Начинаем удаление сайта {siteName}...");
            _ps.ExecutePowerShell($"Remove-WebSite -Name {siteName}");
            if (!string.IsNullOrEmpty(_archivePath))
            {
                if (!Directory.Exists(_archivePath))
                {
                    Directory.CreateDirectory(_archivePath);
                }

                var archivedPath = Path.Combine(_archivePath, siteName);
                Directory.Move(Path.Combine(_sitesPath, siteName), archivedPath);
                Console.WriteLine($"Сайт {siteName} перенесён в архив!");
            }
            else
            {
                Directory.Delete(Path.Combine(_sitesPath, siteName), true);
                Console.WriteLine($"Сайт {siteName} удалён!");
            }
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
                Console.WriteLine($"Добавляем в IIS домен {d} для сайта {siteName}...");
                _ps.ExecutePowerShell($"New-WebBinding -HostHeader {d} -IPAddress * -Name {siteName} -Port 80 -Protocol http -SslFlags 0");
                Console.WriteLine($"Домен {d} добавлен!");
            }
        }
    }
}