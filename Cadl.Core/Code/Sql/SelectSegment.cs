using System;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.Sql
{
    public class SelectSegment : CodeSegment
    {
        private string selectMethod => @"
function #method-name(entity)
{
    return new Promise(function (resolve, reject) {
        connectTo#database();
        #database_connection.on('connect', function (err) {
            if (err) {
                console.log(err);
                reject(err);
            } else {
                console.log('Running ' + '#method-name');

                var query = #sql;
                var request = new Request(query, function (
                    err,
                    rowCount,
                    rows) {
                    console.log('Received ' + rowCount);
                    if (rows) {
                        var data = [];
                        rows.forEach(function (row) {
                            var columns = [];
                            row.forEach(function (column) {
                                columns.push(column.value)
                            });
                            data.push(columns);
                        });
                        console.log(data);
                        resolve(data);
                    }
                    else {
                        console.log('No rows returned. ');
                        resolve([]);
                    });

                #database_connection.execSql(request);
            }
        });
    });
}";

        public SelectSegment(string methodName, string sql, string assignTo, DbInfo dbInfo)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(dbInfo));
            Methods.Add(selectMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", sql)
                .Replace("#database", dbInfo.Database));

            FunctionCode = $"var {assignTo} = await {methodName}();";
        }
    }
}
