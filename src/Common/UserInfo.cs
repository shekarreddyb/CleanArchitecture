using System.Collections.Generic;

namespace CleanArchitecture.Common
{
    public class UserInfo
    {
        public string Name { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }
}
