using System.Linq;
using System.Collections.Generic;
using Cadl.Core.Components;
using Cadl.Core.Extensions;

namespace Cadl.Core.Code.SqlSegments
{
    public enum ReturnAs
    {
        Array,
        Entity,
        Scalar
    }

    public class SelectSegment : Segment
    {
        private const string selectMethod = @"
async function #method-name(#parameters)
{
    return new Promise(function (resolve, reject) {
        var query = #sql;
        var request = new Request(query, function (err, rowCount, rows) {
            if (err) {
                console.log(err);
            }
            else {
                console.log('Received ' + rowCount);
            }
            if (rowCount > 0) {
                if (#scalar) {
                    resolve(rows[0][0].value);
                
                } 
                else if (#entity) {
                    var columns = new Map();
                    rows[0].forEach(function (column) {
                        columns.set(column.metadata.colName, column.value);
                    });
                    var obj = Array.from(columns).reduce((obj, [key, value]) => (
                        Object.assign(obj, { [key]: value }) 
                      ), {});
                    resolve(obj);
                }
                else {
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
            string statement, string assignTo, List<Parameter> parameters, 
            ReturnAs returnAs = ReturnAs.Array)
            : base(indentCount)
        {
            Requires.Add("var Request = require('tedious').Request;");
            DependsOnSegments.Add(new ConnectionSegement(indentCount, sql));
            DependsOnModules.Add("\"tedious\": \"5.0.3\"");
            Methods.Add(selectMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName)
                .Replace("#scalar", (returnAs == ReturnAs.Scalar).ToLowerString())
                .Replace("#entity", (returnAs == ReturnAs.Entity).ToLowerString())
                .Replace("#parameters", string.Join(',', parameters.Select(p => p.Name.Replace("@", ""))))
                .Replace("#add-params", Helper.CreateParameters(parameters))); ;

            FunctionCode = $"var {assignTo} = await {methodName}({string.Join(',', parameters.Select(p => p.Value)).Trim()});";
        }
    }
}
