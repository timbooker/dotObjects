using System;
using dotObjects.Core.Configuration;
using dotObjects.Core.Processing;

namespace dotObjects.Core.UI
{
    [Serializable]
    public class EntityDomain : ComplexDomain
    {
        public EntityDomain(Type type, ProcessURI uri) : base(type, uri, null)
        {
            //EnsureIsEntityType(type, uri);
            ID = type.Name;
        }

        public EntityDomain(Type type, ProcessURI uri, object value)
            : this(type, uri)
        {
            Value = value;
        }

        public EntityDomain(Type type, ProcessURI uri, object value, object instance)
            : this(type, uri, value)
        {
            Instance = instance;
        }

        //private static void EnsureIsEntityType(Type type, ProcessURI uri)
        //{
        //    if (type == null) throw new ArgumentNullException("type");
        //    if (uri == null) throw new ArgumentNullException("uri");

        //    string message = string.Format("The type {0} is not a valid entity type.", type.Name);
        //    if (type.IsPrimitive) throw new ArgumentException(message);
        //    if (CoreSection.Current.FindType(CoreSection.GetTypeName(type)) == null)
        //        throw new ArgumentException(message);
        //}
    }
}