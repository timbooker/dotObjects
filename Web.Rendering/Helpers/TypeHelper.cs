using System;

namespace dotObjects.Web.Rendering.Helpers
{
    public class TypeHelper : RendererHelper
    {
        public TypeHelper(string name) : base(name)
        {
        }

        public static string[] GetEnumNames(Type type)
        {
            if(type.IsEnum)
                return Enum.GetNames(type);
            return Enum.GetNames(type.GetGenericArguments()[0]);
        }

        public static object GetEnumValue(Type type, string name)
        {
            if (type.IsEnum)
                return (int) Enum.Parse(type, name);
            return (int) Enum.Parse(type.GetGenericArguments()[0], name);
        }

        public static object GetEnumObject(Type type, string name)
        {
            if (type.IsEnum)
                return Enum.Parse(type, name);
            return Enum.Parse(type.GetGenericArguments()[0], name);
        }
    }
}