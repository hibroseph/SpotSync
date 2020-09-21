using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class DatabaseMigrationConfig
    {
        public DatabaseMigrationConfig(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public string ConnectionString { get; }
    }
}
