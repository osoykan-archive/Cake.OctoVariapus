namespace Cake.OctoVariapus
{
    public class OctoVariable
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public bool IsSensitive { get; set; }

        public OctoScope Scope { get; set; }
    }
}
