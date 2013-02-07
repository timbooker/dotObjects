using System.Linq;
using dotObjects.Core.Configuration;

namespace dotObjects.Metadata
{
    public class MetadataManager
    {
        public static NamespaceMetadata Root { get; set; }

        internal static void Load(EntityAssembly assembly)
        {
            Root = new NamespaceMetadata(assembly.RootNamespace, assembly.GetTypes());
        }

        public static EntityMetadata Find(string name)
        {
            return (from e in Root.AllEntities where e.Name.Equals(name) select e).FirstOrDefault();
        }
    }
}
