using System;
using Cadl.Core.Components;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.SqlSegments
{
    public class SelectSegment : Segment
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

        public SelectSegment(int indentCount, string methodName, Sql sql, 
            string statement, string assignTo)
            : base(indentCount)
        {
            Requires.Add("var Request = require(\"tedious\").Request;");
            Dependencies.Add(new ConnectionSegement(indentCount, sql));
            Methods.Add(selectMethod
                .Replace("#method-name", methodName)
                .Replace("#sql", statement)
                .Replace("#database", sql.DbName));

            FunctionCode = $"var {assignTo} = await {methodName}();";
        }
    }
}
