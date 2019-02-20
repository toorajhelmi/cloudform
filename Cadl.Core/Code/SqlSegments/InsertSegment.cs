using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;
using Cadl.Core.Extensions;

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
                if (err) {
                    console.log(err);
                }
                else {
                    console.log('Received ' + rowCount);
                }
                if (rowCount > 0) {
                    if (#returns-id) {
                        var id = rows[0][0].value;
                        console.log(id);
                        resolve(id);
                    } else {
                        resolve();
                    }
                }
                else {
                    console.log('Insert Failed.');
                    resolve(null);
                }
            });
        #add-params

        if (#database_connected) {
            #database_connection.execSql(request);
        } else {
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
            string statement, string assignTo, List<Parameter> parameters, bool returnsId)
            : base(indentCount)
        {
            Requires.Add("var Request = require('tedious').Request;");
            DependsOnSegments.Add(new ConnectionSegement(indentCount, sql));
            DependsOnModules.Add("\"tedious\": \"5.0.3\"");
            Methods.Add(insertMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName)
                .Replace("#returns-id", returnsId.ToLowerString())
                .Replace("#parameters", string.Join(',', parameters.Select(p => p.Name.Replace("@", ""))))
                .Replace("#add-params", Helper.CreateParameters(parameters)));
            if (!string.IsNullOrWhiteSpace(assignTo))
            {
                FunctionCode = $"{assignTo} = await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
            }
            else
            {
                FunctionCode = $"await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
            }
        }
    }
}
