using System;
using System.Collections.Generic;

namespace Cloudform.Core.Components
{
    public enum SqlType
    {
        Select,
        Insert,
        Update,
        Delete
    }

    public class Sql : Component
    {
        public string DbName { get; set; }
        public string ServerName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectinString => $"{ServerName}.database.windows.net";
        public List<Table> Tables { get; set; } = new List<Table>();
        public SqlType SqlType { get; set; }
    }
}
