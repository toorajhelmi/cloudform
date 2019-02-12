using System.Linq;
using System.Collections.Generic;
using System.Text;
using Cadl.Core.Parsers;
using Cadl.Core.Code;
using Cadl.Core.Components;
using Cadl.Core.Code.SqlSegments;

namespace Cadl.Core.Interpreters
{
    public class CadlInterpreter
    {
        private int methodCount;
        private List<CodeSegment> segments = new List<CodeSegment>();
        private List<Component> all;
        private Dictionary<string, object> props;
        private Function function;

        public string CompileToJs(Function function, List<Component> all, Dictionary<string, object> props)
        {
            this.function = function;
            this.all = all; 
            var cadl = function.Code; 
            this.props = props;

            for (int i = 0; i < cadl.Count; i++)
            {
                var line = cadl[i];
                if (line.KeyExists("call"))
                {

                }
                else if (line.KeyExists("return"))
                {

                }
                else if (line.KeyExists("sql"))
                {
                    GetScope(cadl.Skip(i).ToList(), out int jump);
                    segments.Add(GetSqlSegement(cadl.Skip(i).Take(jump).ToList()));
                    i += jump;

                }
                else if (line.KeyExists("take"))
                {

                }
                else if (line.KeyExists("code"))
                {

                }
                else
                {
                    segments.Add(new JavaScriptSegment(line.Content));
                }
            }

            return Js();
        }

        private CodeSegment GetSqlSegement(List<Line> scope)
        {
            var db = "";
            var assignTo = "";

            if (scope[0].Parts.Count == 3)
            {
                db = scope[0].Parts[2];
                assignTo = scope[0].Parts[1];
            }
            else if (scope[0].Parts.Count == 2)
            {
                db = scope[0].Parts[1];
            }
            else
            {
                throw new ParsingException(new Error(Error.UnknownSyntax));
            }

            scope[1].EnsureBeginScope();
            scope.Last().EnsureEndScope();
            var statement = Concat(scope, 2, 1);
            var sql = all.OfType<Sql>().First(s => s.DbName == db);
            if (scope[2].Content.Contains("select"))
            {
                var methodName = $"sql_select_{assignTo}_{methodCount++}";
                return new SelectSegment(methodName, sql, statement, assignTo);
            }
            else if (scope[2].Content.Contains("insert"))
            {
                var methodName = $"sql_insert_{assignTo ?? ""}_{methodCount++}".Replace('.', '_'); 
                return new InsertSegment(methodName, sql, statement, assignTo);
            }
            else if (scope[2].Content.Contains("update"))
            {
                var methodName = $"sql_update_{methodCount++}";
                return new UpdateSegment(methodName, sql, statement);
            }
            else if (scope[2].Content.Contains("delete"))
            {
                var methodName = $"sql_delete_{methodCount++}";
                return new DeleteSegment(methodName, sql, statement);
            }
            else
            {
                throw new ParsingException(new Error(Error.UnknownSqlSyntax));
            }
        }

        private string CompileCall(Line line)
        {
            return null;
        }

        private string Js()
        {
            var requires = new List<string>();
            var globalVars = new List<string>();
            var methods = new List<string>();
            var functionCode = new List<string>();

            var dependencies = segments.SelectMany(s => s.Dependencies).GroupBy(d => d.Name);

            foreach (var dependency in dependencies)
            {
                segments.Add(dependency.First());
            }

            foreach (var segment in segments)
            {
                foreach (var require in segment.Requires)
                {
                    if (!requires.Contains(require))
                    {
                        requires.Add(require);
                    }
                }

                foreach (var globalVar in segment.GlobalVars)
                {
                    globalVars.Add(globalVar);
                }

                foreach (var method in segment.Methods)
                {
                    methods.Add(method);
                }

                functionCode.Add(segment.FunctionCode);
            }


            var js = new StringBuilder();

            foreach (var require in requires)
            {
                js.Append(require);
                js.AppendLine();
            }

            js.AppendLine();

            foreach (var globalVar in globalVars)
            {
                js.Append(globalVar);
                js.AppendLine();
            }

            js.AppendLine();

            foreach (var method in methods)
            {
                js.Append(method);
                js.AppendLine();
            }

            js.AppendLine();

            BeginFunction(js);

            foreach (var code in functionCode)
            {
                js.Append(code);
                js.AppendLine();
            }

            EndFunction(js);
            return js.ToString();
        }

        private void GetScope(List<Line> cadl, out int jump)
        {
            jump = 0;
            bool openBraceFound = false;
            for (jump = 0; jump < cadl.Count; jump++)
            {
                var line = cadl[jump];

                if (jump == 1)
                {
                    if (line.EnsureBeginScope())
                    {
                        openBraceFound = true;
                    }
                }

                else if (line.Parts[0].IndexOf('}') != -1)
                {
                    if (!openBraceFound)
                    {
                        throw new ParsingException(new Error(Error.MissingOpenBrace));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void BeginFunction(StringBuilder sb)
        {
            sb.Append("module.exports = async function (context, req) {");
            sb.AppendLine();
            if (function.Trigger == Trigger.Request)
            {
                sb.Append($"var {function.TriggeringMessage} = req.body.{function.TriggeringMessage};");
                sb.AppendLine();
            }
        }

        private void EndFunction(StringBuilder sb)
        {
            sb.Append("}");
        }

        private string Concat(List<Line> lines, int fromBegin, int fromEnd, bool asString = true)
        {
            var sb = new StringBuilder();

            for (int i=fromBegin; i<= lines.Count-fromEnd; i++)
            {
                sb.Append($"'{lines[i].Content}'");
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
