using System;
using System.Collections.Generic;
using Cadl.Core.Interpreters;

namespace Cadl.Core.Code.Sql
{
    public class ConnectionSegement : CodeSegment
    {
        private const string connect = @"
function connectTo#database() {
    if (#database_connection != null) {
        var config = {
            userName: '#username',
            password: '#password',
            server: '#server',
            options: {
                database: '#database',
                encrypt: true,
                rowCollectionOnRequestCompletion: true
            }
        };

        #database_connection = new Connection(config);
    }
}";

        public ConnectionSegement(DbInfo dbInfo)
        {
            Database = dbInfo.Database;

            GlobalVars.Add($"var {dbInfo.Database}_connection;");
            Requires.Add("var Connection = require(\"tedious\").Connection;");
            Methods.Add(connect
                .Replace("#username", dbInfo.Username)
                .Replace("#password", dbInfo.Password)
                .Replace("#server", dbInfo.Server)
                .Replace("#database", dbInfo.Database));
        }

        public override string Name => $"{Database}_Connectiont";
        public string Database { get; set; }
    }
}
