using DbUp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Database
{
    public class DatabaseMigration
    {
        private readonly DatabaseMigrationConfig _config;
        public DatabaseMigration(DatabaseMigrationConfig config)
        {
            _config = config;
        }

        public bool Update()
        {
            try
            {
                EnsureDatabase.For.PostgresqlDatabase(_config.ConnectionString);

                var upgrader = DeployChanges.To.PostgresqlDatabase(_config.ConnectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .Build();

                var result = upgrader.PerformUpgrade();

                return result.Successful;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
