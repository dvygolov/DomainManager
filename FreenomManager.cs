using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Linq;

namespace DomainManager
{
    public class FreenomManager
    {
        private readonly string _apiurl;
        private readonly string _login;
        private readonly string _password;

        public FreenomManager(string apiurl, string login, string password)
        {
            _apiurl = apiurl;
            _login = login;
            _password = password;
        }

        private JObject ExecuteRequest(RestRequest req, bool useBodyParams = false)
        {
            var rc = new RestClient(_apiurl);
            if (useBodyParams)
            {
                req.AddParameter("email", _login);
                req.AddParameter("password", _password);
            }
            else
            {
                req.AddQueryParameter("email", _login);
                req.AddQueryParameter("password", _password);
            }
            var response = rc.Execute(req);
            var json = (JObject)JsonConvert.DeserializeObject(response.Content);
            return json;
        }

        public string SelectDomains()
        {
            var r = new RestRequest($"domain/list", Method.GET);
            r.AddQueryParameter("results_per_page", "100");
            var res = ExecuteRequest(r);
            Console.WriteLine("Выберите домен, введите один или несколько номеров через запятую:");
            var domains = res["domain"].OrderBy(dn => dn["domainname"]).ToList();

            int i = 1;
            foreach (var d in domains)
            {
                Console.WriteLine($"{i}.{d["domainname"]}");
                i++;
            }
            Console.Write("Ваш выбор:");
            var selectedIndexes = Console.ReadLine().Split(',').Select(ind => int.Parse(ind));
            var selectedDomains = string.Join(",", selectedIndexes.Select(ind => domains[ind-1]["domainname"]));
            return selectedDomains;
        }

        public void CancelDomains(string domains)
        {
            foreach (var d in domains.Split(','))
            {
                Console.WriteLine($"Удаляем с Freenom домен {d}...");
                var r = new RestRequest($"domain/delete", Method.DELETE);
                r.AddParameter("domainname", d);
                var res = ExecuteRequest(r, true);
                Console.WriteLine($"Домен {d} успешно удалён!");
            }
        }

        public void ModifyNameServers(string domains, string nameServers)
        {
            foreach (var d in domains.Split(','))
            {
                Console.WriteLine($"Меняем DNS-сервера для домена {d}...");
                var r = new RestRequest($"domain/modify", Method.PUT);
                r.AddParameter("domainname", d);
                nameServers.Split(',').ToList().ForEach(ns =>
                {
                    r.AddParameter("nameserver", ns.Trim());
                });
                var res = ExecuteRequest(r, true);
                Console.WriteLine($"Поменяли DNS-сервера для домена {d}!");
            }
        }
    }
}
