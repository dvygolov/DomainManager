using Microsoft.Extensions.Configuration;
using System;

namespace DomainManager
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var fn=new IISManager();
            fn.GetWebsiteName();
            return;*/
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

            var domainsFilePath = config.GetValue<string>("cloudflare_domains_filepath");
            if (string.IsNullOrEmpty(domainsFilePath))
            {
                Console.WriteLine("Не указан путь к файлу доменов для CloudFlare!");
                return;
            }

            var fnm = new FreenomManager(fnApiPath, fnLogin, fnPassword);
            var im = new IISManager();
            var cfm = new CloudFlareManager(domainsFilePath);

            var operations = new[] { "-a", "-c" };
            string operation = string.Empty;
            string domain = string.Empty;
            string siteName = string.Empty;
            if (args.Length == 0)
            {
                bool operationSelected = false;
                while (!operationSelected)
                {
                    Console.WriteLine("Меню работы с доменами. Выберите операцию:");
                    Console.WriteLine("1.Добавить домен к сайту");
                    Console.WriteLine("2.Удалить забаненый домен");
                    Console.Write("Введите номер:");
                    var opKey = Console.ReadKey();
                    Console.WriteLine();
                    if (!int.TryParse(opKey.KeyChar.ToString(), out _)) continue;
                    var opnum = int.Parse(opKey.KeyChar.ToString()) - 1;
                    if (opnum > operations.Length) continue;
                    if (opnum == 1)
                        siteName = im.GetWebsiteName();
                    operation = operations[opnum];
                    operationSelected = true;
                }
                domain = fnm.SelectDomains();
            }
            else if (args.Length == 2)
            {
                operation = args[0];
                domain = args[1];
            }
            else if (args.Length == 3)
            {
                operation = args[0];
                domain = args[1];
                siteName = args[2];
            }
            else if (args.Length != 2 || args.Length != 3)
            {
                ShowHelp();
                return;
            }

            var arc = new DomainManager(fnm, cfm, im);
            switch (operation)
            {
                case "-c":
                    arc.CancelDomain(domain);
                    break;
                case "-a":
                    arc.AddDomain(domain, siteName);
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Программа для добавления/удаления к сайту IIS доменов by Даниил Выголов.");
            Console.WriteLine("Варианты запуска:");
            Console.WriteLine("Добавление домена");
            Console.WriteLine("-a <DOMAIN_NAME> <IIS_SITENAME>");
            Console.WriteLine("Удаление домена");
            Console.WriteLine("-c <DOMAIN_NAME>");
            Console.WriteLine("Примечание: можно указать несколько доменов через запятую БЕЗ ПРОБЕЛА.");
        }
    }
}
