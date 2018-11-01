using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace CosmosMongoDbConnection
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new MongoClient("[your connectionstring]");
            IMongoDatabase db = client.GetDatabase("somedb");
            db.DropCollection("people");
            IMongoCollection<Person> collection = db.GetCollection<Person>("people");
            collection.InsertOne(new Person
            {
                FirstName = "Bill",
                LastName = "Gates",
                Interests = new List<string>
                {
                    "Microsoft",
                    "Philanthropy",
                    "Being rich"
                }
            });
            var people = collection.Find(_ => true).ToList();
            foreach (var p in people)
            {
                Console.WriteLine($"{p.Id} - {p.FirstName} {p.LastName} - {string.Join(',', p.Interests)}");
            }

            collection.InsertMany(new[]
            {
                new Person
                {
                    FirstName = "Steve",
                    LastName = "Ballmer",
                    Interests = new List<string>
                    {
                        "Basketball",
                        "Developers",
                        "Being rich"
                    }
                },
                new Person
                {
                    FirstName = "Satya",
                    LastName = "Nadella",
                    Interests = new List<string>
                    {
                        "Open source",
                        "Microsoft"
                    }
                }
            });

            Console.WriteLine();

            var builder = new FilterDefinitionBuilder<Person>();
            var definition = builder.AnyEq(p => p.Interests, "Being rich");
            people = collection.Find(definition).ToList();
            foreach (var p in people)
            {
                Console.WriteLine($"{p.Id} - {p.FirstName} {p.LastName} - {string.Join(',', p.Interests)}");
            }

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }

    internal class Person
    {
        public ObjectId Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Interests { get; set; }
        public DateTime? DateOfBirth { get; set; } // Defaults to null
        public DateTime JoinDate { get; set; } // Defaults to 01-01-0001
    }
}
