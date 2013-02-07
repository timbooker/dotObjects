using System;
using System.Collections;
using System.IO;
using System.Reflection;
using dotObjects.Core.Configuration;
using dotObjects.Core.Serialization;

namespace dotObjects.Web.Serialization
{
    internal static class JsonSerializer
    {
        public static void SerializeObject(object instance, TextWriter writer)
        {
            if (instance == null)
                writer.Write("null");
            else
            {
                Type objectType = instance.GetType();
                if (objectType.IsEnum || CoreSection.IsSystemType(objectType))
                    writer.Write("\"" + instance + "\"");
                else
                    SerializeComplexObject(instance, writer, objectType);
            }
        }

        private static void SerializeComplexObject(object instance, TextWriter writer, Type objectType)
        {
            writer.Write("{");
            PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo prop = properties[i];
                object[] ns = prop.GetCustomAttributes(typeof(NonSerializableAttribute), true);
                if (ns.Length == 0)
                {
                    SerializeComplexObjectProperty(instance, prop, writer);
                    if (i < properties.Length - 1)
                        writer.Write(",");
                }
            }
            writer.Write("}");
        }

        private static void SerializeComplexObjectProperty(object instance, PropertyInfo prop, TextWriter writer)
        {
            if (prop.PropertyType.GetInterface(typeof(ICollection).Name) != null)
            {
                writer.Write("\"" + prop.Name + "\" :");
                SerializeCollection((ICollection)prop.GetValue(instance, null), writer);
            }
            else if (prop.PropertyType.IsEnum || CoreSection.IsSystemType(prop.PropertyType))
            {
                writer.Write("\"" + prop.Name + "\" : \"" + prop.GetValue(instance, null) + "\"");
            }
            else if (prop.PropertyType.Equals(typeof(Type)))
            {
                writer.Write("\"" + prop.Name + "\" : \"" + ((Type)prop.GetValue(instance, null)).FullName + "\"");
            }
            else
            {
                writer.Write("\"" + prop.Name + "\" :");
                SerializeObject(prop.GetValue(instance, null), writer);
            }
        }

        private static void SerializeCollection(ICollection collection, TextWriter writer)
        {
            writer.Write("[");
            object[] items = new object[collection.Count];
            collection.CopyTo(items, 0);

            for (int i = 0; i < items.Length; i++)
            {
                SerializeObject(items[i], writer);
                if (i < items.Length - 1)
                    writer.Write(",");
            }
            writer.Write("]");
        }
    }
}