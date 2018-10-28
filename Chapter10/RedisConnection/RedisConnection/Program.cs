using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RedisConnection
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Examples using the Microsoft Extensions");
            Console.WriteLine();
            // For MVC Core applications.
            //services.AddDistributedRedisCache(option =>
            //{
            //    option.Configuration = "[your connectionstring]";
            //    option.InstanceName = "[your DNS name]";
            //});
            var redisOptions = new RedisCacheOptions
            {
                Configuration = "[your DNS name].redis.cache.windows.net:6380,password=[your password],ssl=True,abortConnect=False",
                InstanceName = "[your DNS name]"
            };
            var options = Options.Create(redisOptions);
            // This would be injected in MVC Core.
            IDistributedCache cache = new RedisCache(options);
            cache.SetString("name", "Bill Gates");
            Console.WriteLine($"Hi, my name is {cache.GetString("name")}");

            cache.SetString("name", "Steve Ballmer", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, 3)
            });

            Console.WriteLine($"Hi, my name is {cache.GetString("name") ?? "unset"}");
            Thread.Sleep(3000);
            Console.WriteLine($"Hi, my name is {cache.GetString("name") ?? "unset"}");

            Console.WriteLine();

            // Store object as JSON.
            var person = new Person { Name = "Satya Nadella" };
            string json = JsonConvert.SerializeObject(person);
            cache.SetString("person", json);

            // Retrieve object as JSON.
            json = cache.GetString("person");
            person = JsonConvert.DeserializeObject<Person>(json);
            Console.WriteLine($"Hi, my name is {person.Name}");

            // ServiceStack.Redis.Core
            Console.WriteLine();
            Console.WriteLine("Examples using ServiceStack");

            var connectionstring = "redis://[your password]@[your DNS name].redis.cache.windows.net:6380?ssl=true";
            using (var manager = new RedisManagerPool(connectionstring))
            using (var client = manager.GetClient())
            {
                // Set a counter and increment it by 1 and 3.
                client.Set("counter", 1);
                Console.WriteLine($"The counter is at {client.Get<int>("counter")}");
                client.IncrementValue("counter");
                Console.WriteLine($"The counter is now at {client.Get<int>("counter")}");
                client.IncrementValueBy("counter", 3);
                Console.WriteLine($"The counter is now at {client.Get<int>("counter")}");

                Console.WriteLine();

                // Set multiple values at once.
                var dict = new Dictionary<string, object>()
                {
                    { "name", "Bill Gates" },
                    { "company", "Microsoft" },
                    { "dateofbirth", new DateTime(1955, 10, 28) },
                    { "networth", 93800000000 }
                };
                client.SetAll(dict);

                // Get multiple values at once.
                var values = client.GetAll<object>(new[] { "name", "company", "dateofbirth", "networth" });
                foreach (var pair in values)
                {
                    Console.WriteLine($"{pair.Key} - {pair.Value}");
                }

                Console.WriteLine($"Hi, my name is {client.Get<string>("name")}");
                Console.WriteLine($"I work for {client.Get<string>("company")}");
                Console.WriteLine($"I was born on {client.Get<DateTime>("dateofbirth").ToLongDateString()}");
                Console.WriteLine($"And my net worth is {client.Get<long>("networth")}");

                Console.WriteLine();

                // Store a list of objects and retrieve it.
                var ceos = new List<Person>
                {
                    new Person { Name = "Bill Gates" },
                    new Person { Name = "Steve Ballmer" },
                    new Person { Name = "Satya Nadella" }
                };
                client.Set("ceos", ceos);
                ceos = client.Get<List<Person>>("ceos");
                foreach (Person p in ceos)
                {
                    Console.WriteLine($"CEO {p.Name}");
                }

            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        private class Person
        {
            public string Name { get; set; }
        }
    }
}
