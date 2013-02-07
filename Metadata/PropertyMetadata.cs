using System.Reflection;

namespace dotObjects.Metadata
{
    public class PropertyMetadata
    {
        private PropertyInfo UnderlineProperty { get; set; }

        internal PropertyMetadata(PropertyInfo property)
        {
            UnderlineProperty = property;
        }
    }
}
