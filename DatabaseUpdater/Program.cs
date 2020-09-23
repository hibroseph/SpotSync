using Database;
using Microsoft.Extensions.Configuration;
using System;

namespace DatabaseUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Building configuration");

            IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

            if (string.IsNullOrWhiteSpace(configuration["DatabaseConnection"]))
            {
                throw new Exception("Database name is null");
            }

            Console.WriteLine("Done building configuration");
            Console.WriteLine("Updating database");

            DatabaseMigration migration = new DatabaseMigration(new DatabaseMigrationConfig(configuration["DatabaseConnection"]));

            migration.Update();

            Console.WriteLine("Done updating database");

        }
    }
}
