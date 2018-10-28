using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AzureSqlEntityFrameworkConnection
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var context = new MigrationDbContext())
            {
                context.Database.Migrate();
                var persons = context.Persons.ToList();
                foreach (Person p in persons)
                {
                    Console.WriteLine($"{p.Id} - {p.FirstName} {p.LastName}");
                }
            }
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }

    internal class MigrationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:[your server].database.windows.net,1433;" +
                "Initial Catalog=migrationdb;Persist Security Info=False;" +
                "User ID=[your username];Password=[your password];" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable(nameof(Person));
            var addressTable = modelBuilder.Entity<Address>().ToTable(nameof(Address));
            addressTable.Property(a => a.Street).HasColumnType("nvarchar(255)");
            addressTable.Property(a => a.Number).HasColumnType("varchar(8)");
            addressTable.Property(a => a.Postalcode).HasColumnType("char(6)");
            addressTable.Property(a => a.City).HasColumnType("nvarchar(255)");
            addressTable.Property(a => a.Country).HasColumnType("char(2)");
        }

        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
    }

    internal class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }
    }

    internal class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Postalcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
