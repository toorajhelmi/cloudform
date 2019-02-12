using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Cadl.Core.Components;

namespace Cadl.Core.Deployers
{
    public class SqlDeployer
    {
        public static void PopulateSql(Sql sql, Dictionary<string, object> credentials)
        {
            var cb = new SqlConnectionStringBuilder
            {
                DataSource = $"{sql.ServerName}.database.windows.net",
                UserID = credentials["sql_admin"].ToString(),
                Password = credentials["sql_password"].ToString(),
                InitialCatalog = $"{sql.DbName}"
            };

            try
            {
                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    connection.Open();

                    foreach (var table in sql.Tables)
                    {
                        try
                        {
                            SubmitNonQuery(
                                connection,
                                $"Creating table {table.Name}",
                                CreateTable(table));

                            if (table.Insests != null)
                            {
                                SubmitNonQuery(
                                    connection,
                                    $"Populating table {table.Name}",
                                    Insert(table));
                            }
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine($"Creating table '{table.Name}' failed.\nError: {x.Message}");
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine($"Creating the DB '{sql.DbName}' failed.\nError: {x.Message}");
            }
        }

        static string CreateTable(Table table)
        {
            return $@"
                DROP TABLE IF EXISTS {table.Name};
                CREATE TABLE {table.Name}
                (
                    {table.TSql}
                );";
        }

        static string Insert(Table table)
        {
            return $@"
                INSERT INTO {table.Name} 
                {table.Insests};";
        }

        private static void SubmitNonQuery(
            SqlConnection connection,
            string tsqlPurpose,
            string tsqlSourceCode,
            string parameterName = null,
            string parameterValue = null
            )
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                if (parameterName != null)
                {
                    command.Parameters.AddWithValue(  // Or, use SqlParameter class.
                        parameterName,
                        parameterValue);
                }
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
            }
        }
    }
}
