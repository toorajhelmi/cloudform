using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;

namespace Cadl.Core.Code.SqlSegments
{
    public class InsertSegment : Segment
    {
        private const string insertMethod = @"
async function #method-name(#parameters)
{
    return new Promise(function (resolve, reject) {
        var query = #sql;
        var request = new Request(query, function (
            err,
            rowCount,
            rows) {
            console.log('Received ' + rowCount);
                if (rows) {
                    var id = rows[0][0].value;
                    console.log(id);
                    resolve(id);
                }
                else {
                    console.log('Insert Failed.');
                    resolve(null);
                }
            });
        if (#database_connected) {
            #database_connection.execSql(request);
        } else {
            request#add-params;
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

        public InsertSegment(int indentCount, string methodName, Sql sql, 
            string statement, string entityId, List<Parameter> parameters)
            : base(indentCount)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(indentCount, sql));
            Methods.Add(insertMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName)
                .Replace("#parameters", string.Join(',', parameters.Select(p => p.Name.Replace("@", ""))))
                .Replace("#add-params", Helper.CreateParameters(parameters)));
            if (!string.IsNullOrWhiteSpace(entityId))
            {
                FunctionCode = $"{entityId} = await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
            }
            else
            {
                FunctionCode = $"await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
            }
        }
    }
}
