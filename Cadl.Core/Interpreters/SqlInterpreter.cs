using System.Linq;
using System.Collections.Generic;
using System.Text;
using Cadl.Core.Code;
using Cadl.Core.Code.SqlSegments;
using Cadl.Core.Components;
using Cadl.Core.Parsers;

namespace Cadl.Core.Interpreters
{
    public class SqlInterpreter
    {
        public static Segment GetSqlSegement(List<Component> components, List<Line> scope,
            int indentCount, ref int methodCount)
        {
            var db = "";
            var assignTo = "";
            var isScalar = false;

            if (scope[0].Content.IndexOf('=') == -1 && scope[0].Parts.Count == 3)
            {
                db = scope[0].Parts[1];
                assignTo = scope[0].Parts[2];
                isScalar = false;
            }
            else if (scope[0].Content.IndexOf('=') == -1 && scope[0].Parts.Count == 2)
            {
                db = scope[0].Parts[1];
                isScalar = false;
            }
            else if (scope[0].Content.IndexOf('=') != -1 && scope[0].Parts.Count == 4)
            {
                assignTo = scope[0].Parts[0];
                db = scope[0].Parts[3];
                isScalar = true;
            }
            else
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }

            scope[1].EnsureBeginScope();
            scope.Last().EnsureEndScope();
            var statement = Concat(scope, 2, 1);
            var sql = components.OfType<Sql>().First(s => s.DbName == db);
            if (scope[2].Content.Contains("select"))
            {
                sql.SqlType = SqlType.Select;
                var methodName = $"sql_select_{assignTo}_{methodCount++}";
                statement = IncludeSqlParamers(statement, sql, out List<Parameter> parameters);
                return new SelectSegment(indentCount, methodName, sql, statement, assignTo, parameters,
                    isScalar);
            }
            else if (scope[2].Content.Contains("insert"))
            {
                sql.SqlType = SqlType.Insert;
                var methodName = $"sql_insert_{assignTo ?? ""}_{methodCount++}".Replace('.', '_');
                statement = IncludeSqlParamers(statement, sql, out List<Parameter> parameters);
                return new InsertSegment(indentCount, methodName, sql, statement, 
                    assignTo, parameters, assignTo != "");
            }
            else if (scope[2].Content.Contains("update"))
            {
                sql.SqlType = SqlType.Update;
                var methodName = $"sql_update_{methodCount++}";
                statement = IncludeSqlParamers(statement, sql, out List<Parameter> parameters);
                return new UpdateSegment(indentCount, methodName, sql, statement, parameters);
            }
            else if (scope[2].Content.Contains("delete"))
            {
                sql.SqlType = SqlType.Delete;
                var methodName = $"sql_delete_{methodCount++}";
                statement = IncludeSqlParamers(statement, sql, out List<Parameter> parameters);
                return new DeleteSegment(indentCount, methodName, sql, statement, parameters);
            }
            else
            {
                throw new ParsingException(new Error(Error.UnknownSqlSyntax));
            }
        }

        private static string IncludeSqlParamers(string statement, Sql sql, 
        out List<Parameter> parameters)
        {
            parameters = new List<Parameter>();
            var types = new List<string>();

            var startIndex = 0;
            while (statement.IndexOf('@', startIndex) != -1)
            {
                startIndex = statement.IndexOf('@', startIndex);
                var endIndex = statement.IndexOfAny(new []{' ', ','}, startIndex);
                if (endIndex == -1)
                {
                    endIndex = statement.Length;
                }

                var parameter = statement.Substring(startIndex, endIndex - startIndex).Replace("@", "");
                var name = parameter.Replace('.', '_');
                var type = "";
                for (int i = startIndex-2; statement[i] != '[' && i>=0; i--)
                {
                    if ("]{}() \n,.".Contains(statement[i]) || (i == 0 && statement[i] != '['))
                    {
                        throw new ParsingException(new Error(Error.ParameterTypeMissing, parameter));
                    }

                    type = statement[i] + type;
                    types.Add(type);
                }

                parameters.Add(new Parameter(name, Table.ToTediousTypes(type), parameter));
                statement = statement.Replace(parameter, name);
                startIndex++;
            }

            foreach (var type in types.Distinct())
            {
                statement = statement.Replace($"[{type}]", "");
            }

            return statement;
        }

        private static string Concat(List<Line> lines, int fromBegin, int fromEnd, bool asString = true)
        {
            var sb = new StringBuilder();

            for (int i = fromBegin; i <= lines.Count - fromEnd; i++)
            {
                sb.Append($"'{lines[i].Content} '");
                if (i < lines.Count - fromEnd)
                {
                    sb.Append(" +");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
