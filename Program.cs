using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DomainManager
{
    class Program
    {
        static void Main()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var fnLogin = config.GetValue<string>("freenom_login");
            var fnPassword = config.GetValue<string>("freenom_password");
            var fnApiPath = config.GetValue<string>("freenom_apipath");
            if (string.IsNullOrEmpty(fnLogin) ||
                string.IsNullOrEmpty(fnPassword) ||
                string.IsNullOrEmpty(fnApiPath))
            {
                Console.WriteLine("Не указаны нужные настройки Freenom в конфигурационном файле!");
                return;
            }

            bool useCloudFlare = config.GetValue<bool>("use_cloudflare");
            string domainsFilePath = string.Empty;
            if (useCloudFlare)
            {
                domainsFilePath = config.GetValue<string>("cloudflare_domains_filepath");
                if (string.IsNullOrEmpty(domainsFilePath))
                {
                    Console.WriteLine("Не указан путь к файлу доменов для CloudFlare!");
                    return;
                }
            }



            var iisSitesPath = config.GetValue<string>("iis_sites_path");
            if (string.IsNullOrEmpty(iisSitesPath))
            {
                Console.WriteLine("Не указан путь к папке сайтов IIS!");
                return;
            }
            if (!Directory.Exists(iisSitesPath))
            {
                Console.WriteLine($"Папка сайтов для IIS не существует по указанному пути {iisSitesPath}!");
                return;
            }
            var iisArchivePath = config.GetValue<string>("iis_archive_path");

            var fnm = new FreenomManager(fnApiPath, fnLogin, fnPassword);
            var im = new IISManager(iisSitesPath, iisArchivePath);
            var cfm = useCloudFlare ? new CloudFlareManager(domainsFilePath) : null;
            var dm = new DomainManager(fnm, cfm, im);

            bool operationSelected = false;
            int opnum = 0;
            while (!operationSelected)
            {
                Console.WriteLine("-----Domains Manager by Yellow Web-----");
                Console.WriteLine("Выберите операцию:");
                Console.WriteLine("1.Создать новый сайт");
                Console.WriteLine("2.Добавить домен к сайту");
                Console.WriteLine("3.Удалить забаненый домен");
                Console.WriteLine("4.Удалить сайт");
                Console.WriteLine("---------------------------------------");
                Console.Write("Введите номер:");
                var opKey = Console.ReadKey();
                Console.WriteLine();
                if (!int.TryParse(opKey.KeyChar.ToString(), out _)) continue;
                opnum = int.Parse(opKey.KeyChar.ToString());
                if (opnum > 4 || opnum < 1) continue;
                operationSelected = true;
            }
            switch (opnum)
            {
                case 1:
                    dm.CreateNewSite();
                    break;
                case 2:
                    dm.AddDomain();
                    break;
                case 3:
                    dm.CancelDomain();
                    break;
                case 4:
                    dm.DeleteSite();
                    break;
            }

        }
    }
}
