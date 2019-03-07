using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Cloudform.Core.Arctifact;
using Cloudform.Core.Components;
using Cloudform.Core.Reporting;

namespace Cloudform.Core.Deployers
{
    public class SqlDeployer
    {
        private Sql sql;
        private Factory factory;
        private IEventLogger eventLogger;

        public SqlDeployer(Sql sql, Factory factory, IEventLogger eventLogger)
        {
            this.sql = sql;
            this.factory = factory;
            this.eventLogger = eventLogger;
        }

        public void PopulateSql()
        {
            var cb = new SqlConnectionStringBuilder
            {
                DataSource = $"{sql.ServerName}.database.windows.net",
                UserID = sql.Username,
                Password = sql.Password,
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

                            if (table.Inserts != null)
                            {
                                SubmitNonQuery(
                                    connection,
                                    $"Populating table {table.Name}",
                                    Insert(table));
                            }
                        }
                        catch (Exception x)
                        {
                            eventLogger.Log(factory.BuildId, $"Creating table '{table.Name}' failed.\nError: {x.Message}");
                        }
                    }
                }
            }
            catch (Exception x)
            {
                eventLogger.Log(factory.BuildId, $"Creating the DB '{sql.DbName}' failed.\nError: {x.Message}");
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
                {table.Inserts};";
        }

        private void SubmitNonQuery(
            SqlConnection connection,
            string tsqlPurpose,
            string tsqlSourceCode,
            string parameterName = null,
            string parameterValue = null
            )
        {
            eventLogger.NextLine(factory.BuildId);
            eventLogger.Log(factory.BuildId, "=================================");
            eventLogger.Log(factory.BuildId, $"T-SQL to {tsqlPurpose}...");

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                if (parameterName != null)
                {
                    command.Parameters.AddWithValue(  // Or, use SqlParameter class.
                        parameterName,
                        parameterValue);
                }
                int rowsAffected = command.ExecuteNonQuery();
                eventLogger.Log(factory.BuildId, $"{rowsAffected} = rows affected.");
            }
        }
    }
}
