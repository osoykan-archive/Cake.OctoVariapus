using System.Collections.Generic;

namespace Cake.OctoVariapus
{
    public class OctoVariable
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public bool IsSensitive { get; set; }

        public bool IsEditable { get; set; }

        public List<OctoScope> Scopes { get; set; }
    }
}
