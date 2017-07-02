using System.Collections.Generic;

namespace Cake.OctoVariapus
{
    public class OctoScope
    {
        public OctoScope()
        {
            Values = new List<string>();
        }

        public string Name { get; set; }

        public List<string> Values { get; set; }
    }
}
