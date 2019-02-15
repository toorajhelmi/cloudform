using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;

namespace Cadl.Core.Code.SqlSegments
{
    public class SelectSegment : Segment
    {
        private const string selectMethod = @"
async function #method-name(#parameters)
{
    return new Promise(function (resolve, reject) {
        var query = #sql;
        var request = new Request(query, function (err, rowCount, rows) {
            console.log('Received ' + rowCount);
            if (rows) {
                if (#is-scalar) {
                    resolve(rows[0][0].value);
                } else {
                    var data = [];
                    rows.forEach(function (row) {
                        var columns = new Map();
                        row.forEach(function (column) {
                            columns.set(column.metadata.colName, column.value);
                        });
                        var obj = Array.from(columns).reduce((obj, [key, value]) => (
                            Object.assign(obj, { [key]: value }) 
                          ), {});
                        console.log(obj);
                        data.push(obj);
                    });
                    resolve(data)
                }
            }
            else {
                resolve(null);
            }
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

        public SelectSegment(int indentCount, string methodName, Sql sql, 
            string statement, string assignTo, List<Parameter> parameters, bool isScalar = false)
            : base(indentCount)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(indentCount, sql));
            Methods.Add(selectMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName)
                .Replace("#is-scalar", isScalar.ToString())
                .Replace("#parameters", string.Join(',', parameters.Select(p => p.Name.Replace("@", ""))))
                .Replace("#add-params", Helper.CreateParameters(parameters))); ;

            FunctionCode = $"var {assignTo} = await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
        }
    }
}
