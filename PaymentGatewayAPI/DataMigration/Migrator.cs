using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace PaymentGatewayAPI.DataMigration
{
    public static class Migrator
    {
        public static IApplicationBuilder Migrate(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.ListMigrations();
            runner.MigrateUp(20201025); 
            return app;
        }

        public static void EnsureDatabase(string connectionString)
        {
            var conn = new NpgsqlConnectionStringBuilder(connectionString);
            var connectionStringWithoutDb = $"Host={conn.Host};User Id={conn.Username};Password={conn.Password};";
            if (!DatabaseExists(connectionStringWithoutDb, conn.Database))
                CreateDatabase(connectionStringWithoutDb, conn.Database);
        }

        private static bool DatabaseExists(string connectionStringWithoutDb, string dbName)
        {
            string script = $"SELECT COUNT(*) AS COUNT FROM pg_database WHERE datname= '{dbName}'";
            using NpgsqlConnection connection = new NpgsqlConnection(connectionStringWithoutDb);
            NpgsqlCommand command = new NpgsqlCommand(script, connection);
            connection.Open();
            var result = (long)command.ExecuteScalar();
            return result > 0;
        }

        private static void CreateDatabase(string connectionStringWithoutDb, string dbName)
        {
            string createScript = $"CREATE DATABASE \"{dbName}\"  WITH OWNER = postgres ENCODING = 'UTF8' " +
                $"LC_COLLATE = 'C' LC_CTYPE = 'C' TABLESPACE = pg_default CONNECTION LIMIT = -1 TEMPLATE template0; ";
            using NpgsqlConnection connection = new NpgsqlConnection(connectionStringWithoutDb);
            NpgsqlCommand command = new NpgsqlCommand(createScript, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
