using System;
using System.Collections.Generic;

namespace Cadl.Core.Settings
{
    public enum TargetCloud
    {
        Azure,
        Aws,
        Gcp
    }

    public class CloudConfiguration
    {
        public Dictionary<string, object> GlobalConfig => new Dictionary<string, object> {
                { "sql_admin", "admin2020" },
                { "sql_password", "password1!"}
            };

        public Dictionary<string, object> AzureConfig => new Dictionary<string, object> {
                { "client_secret", "){BQ6{h>?-a568OG#))Y-n5V!|[b(^&" },
                { "subscription_id", "9a4fe1a5-274e-4c67-8321-8a55ec1ea64d" },
                { "client_id", "7ffb12bc-357e-46e5-83e2-7231372561a4" },
                { "tenant_id", "bafa704d-560b-4ee8-9563-c265cae5ffe6" },
                { "azure_user", "tooraj@me.com" },
                { "azure_pass", "Hapalu2015!" },
            };
    }
}
