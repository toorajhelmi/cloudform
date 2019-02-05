using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Cadl.Core.Components;

namespace Cadl.Core.Deployers
{
    public class SqlDeployer
    {
        public static void PopulateSql(Sql sql, Dictionary<string, string> credentials)
        {
            var cb = new SqlConnectionStringBuilder();
            cb.DataSource = $"{sql.DbName}server.database.windows.net";
            cb.UserID = credentials["sql_admin"];
            cb.Password = credentials["sql_password"];
            cb.InitialCatalog = $"{sql.DbName}";

            using (var connection = new SqlConnection(cb.ConnectionString))
            {
                foreach (var table in sql.Tables)
                {
                    SubmitNonQuery(
                        connection,
                        $"Creating table {table.Name}",
                        CreateTable(table));
                }
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

        static string Build_4_Tsql_UpdateJoin()
        {
            return @"
                DECLARE @DName1  nvarchar(128) = @csharpParmDepartmentName;  --'Accounting';

                -- Promote everyone in one department (see @parm...).
                UPDATE empl
                SET
                    empl.EmployeeLevel += 1
                FROM
                    tabEmployee   as empl
                INNER JOIN
                    tabDepartment as dept ON dept.DepartmentCode = empl.DepartmentCode
                WHERE
                    dept.DepartmentName = @DName1;
            ";
                }

        static string Build_5_Tsql_DeleteJoin()
        {
            return @"
                DECLARE @DName2  nvarchar(128);
                SET @DName2 = @csharpParmDepartmentName;  --'Legal';

                -- Right size the Legal department.
                DELETE empl
                FROM
                    tabEmployee   as empl
                INNER JOIN
                    tabDepartment as dept ON dept.DepartmentCode = empl.DepartmentCode
                WHERE
                    dept.DepartmentName = @DName2

                -- Disband the Legal department.
                DELETE tabDepartment
                    WHERE DepartmentName = @DName2;
            ";
        }

        static string Build_6_Tsql_SelectEmployees()
        {
            return @"
                -- Look at all the final Employees.
                SELECT
                    empl.EmployeeGuid,
                    empl.EmployeeName,
                    empl.EmployeeLevel,
                    empl.DepartmentCode,
                    dept.DepartmentName
                FROM
                    tabEmployee   as empl
                LEFT OUTER JOIN
                    tabDepartment as dept ON dept.DepartmentCode = empl.DepartmentCode
                ORDER BY
                    EmployeeName;
            ";
        }

        public static void Select(SqlConnection connection)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("Now, SelectEmployees (6)...");

            string tsql = Build_6_Tsql_SelectEmployees();

            using (var command = new SqlCommand(tsql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("{0} , {1} , {2} , {3} , {4}",
                            reader.GetGuid(0),
                            reader.GetString(1),
                            reader.GetInt32(2),
                            (reader.IsDBNull(3)) ? "NULL" : reader.GetString(3),
                            (reader.IsDBNull(4)) ? "NULL" : reader.GetString(4));
                    }
                }
            }
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
