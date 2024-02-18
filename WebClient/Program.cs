using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace WebClient
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            Random random = new Random();
            Console.WriteLine("-f --id - найти пользовтеля по id");
            Console.WriteLine("-mk --fn --ln - сгенерировать нового пользователя");
            Console.WriteLine("-all");
            Console.WriteLine("-end - завершить работу");
            while (true)
            {
                try
                {
                    var resp = Console.ReadLine();
                    if (resp.Equals("-end"))
                        break;
                    int pos = resp.IndexOf("--id");
                    if (resp.Contains("-f") && pos >= 0)
                    {
                        var id_str = resp.Substring(pos + 4).Trim();
                        int id = -1;
                        if (int.TryParse(id_str, out id))
                        {
                            var user = await GetCustomer(id, client);
                            Console.WriteLine(String.Format("{0} {1} {2}", user.Id, user.Firstname, user.Lastname));
                            continue;
                        }
                    }

                    if (resp.Contains("-mk") && resp.Contains("--fn") && resp.Contains("--ln"))
                    {
                        int pos1 = resp.IndexOf("--fn");
                        int pos2 = resp.IndexOf("--ln");
                        var fn = resp.Substring(pos1 + 4, pos2 - pos1 - 4).Trim();
                        var ln = resp.Substring(pos2 + 4).Trim();

                        var user = new Customer()
                        {
                            Id = random.Next(10, 15),
                            Firstname = fn,
                            Lastname = ln
                        };
                        var result = await CreateCustomer(user, client);
                        if (result > 0)
                            Console.WriteLine(String.Format("Пользователь успешно создан id = {0}", result));
                        else
                            Console.WriteLine("Произошла ошибка при создании пользователя");
                        continue;
                    }
                    if (resp.Contains("-all"))
                    {
                        var lst = await GetAll(client);
                        foreach (var cust in lst)
                            Console.WriteLine($"{cust.Id}\t{cust.Firstname}\t{cust.Lastname}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return;
        }
    
    
        private static async Task<Customer> GetCustomer(long id, HttpClient client)
        {           
            var url = Method.Host + id.ToString();
            var resp = await client.GetAsync(url);
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadFromJsonAsync<Customer>();
            else
                throw new Exception($"{resp.StatusCode }:{ resp.ReasonPhrase}");
        }
        private async  static Task<int> CreateCustomer(Customer user, HttpClient client)
        {
            var request = new CustomerCreateRequest() {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Id = user.Id
            };
            
            var url = Method.Host + Method.CreateCustomer;
            var resp = await client.PostAsJsonAsync(url, request);
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadFromJsonAsync<int>();
            else
                throw new Exception($"{resp.StatusCode }:{ resp.ReasonPhrase}");
        }
        private static async Task<List<Customer>> GetAll(HttpClient client)
        {
            var url = Method.Host + Method.GetllCustomers;
            var resp = await client.GetAsync(url);
            if (resp.IsSuccessStatusCode)
                return await resp.Content.ReadFromJsonAsync<List<Customer>>();
            else
                throw new Exception($"{resp.StatusCode}:{resp.ReasonPhrase}");
        }
    }
}