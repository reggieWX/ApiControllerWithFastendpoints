using System.Collections.Generic;

namespace ApiControllerWithFastendpoints.Authentication
{
    public class ApiKeys : List<ApiKey> { }

    public class ApiKey
    {
        public string Owner { get; set; }
        public string Key { get; set; }
        public string Roles { get; set; }
    }
}
