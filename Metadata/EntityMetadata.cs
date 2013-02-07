using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace dotObjects.Metadata
{
    public class EntityMetadata
    {
        private List<PropertyMetadata> properties;
        private List<ConstructorMetadata> constructors;
        private List<StaticMethodMetadata> staticMethods;
        private List<InstanceMethodMetadata> instanceMethods;
        public string Name { get; set; }
        public Type UnderlineType { get; set; }
        
        public List<PropertyMetadata> Properties
        {
            get { return properties ?? (properties = new List<PropertyMetadata>()); }
        }

        public List<ConstructorMetadata> Constructors
        {
            get { return constructors ?? (constructors = new List<ConstructorMetadata>()); }
        }

        public List<StaticMethodMetadata> StaticMethods
        {
            get { return staticMethods ?? (staticMethods = new List<StaticMethodMetadata>()); }
        }

        public List<InstanceMethodMetadata> InstanceMethods
        {
            get { return instanceMethods ?? (instanceMethods = new List<InstanceMethodMetadata>()); }
        }

        internal EntityMetadata(Type type)
        {
            Name = type.FullName;
            UnderlineType = type;

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                Properties.Add(new PropertyMetadata(property));

            foreach (var constructor in type.GetConstructors(BindingFlags.Public).Where(c => c.GetParameters().Any()))
                Constructors.Add(new ConstructorMetadata(constructor));

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod))
                StaticMethods.Add(new StaticMethodMetadata(method));

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly))
                InstanceMethods.Add(new InstanceMethodMetadata(method));
        }
    }
}
