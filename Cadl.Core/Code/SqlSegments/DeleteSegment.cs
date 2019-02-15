using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;

namespace Cadl.Core.Code.SqlSegments
{
    public class DeleteSegment : Segment
    {
        private const string updateMethod = @"
async function #method-name(#parameters)
{
    return new Promise(function (resolve, reject) {
        var query = #sql;
        var request = new Request(query, function (
            err,
            rowCount,
            rows) {
                resolve();
            });
        #add-params

        if (#database_connected) {
            #database_connection.execSql(request);
        } else {
            connectTo#database();
            #database_connection.on('connect', function (err) {
                if (err) {
                    console.log(err);
                    reject(err);
                } else {
                    console.log('Running ' + '#method-name');
                    #database_connection.execSql(request);
                }
            });
        }
    });
}";

        public DeleteSegment(int indentCount, string methodName, Sql sql, string statement,
            List<Parameter> parameters)
            : base(indentCount)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(indentCount, sql));
            Methods.Add(updateMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName)
                .Replace("#parameters", string.Join(',', parameters.Select(p => p.Name.Replace("@", ""))))
                .Replace("#add-params", Helper.CreateParameters(parameters))); ;
            FunctionCode = $"await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
        }
    }
}
