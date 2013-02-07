using System;
using System.Collections.Generic;
using System.Reflection;
using dotObjects.Core.Configuration;
using dotObjects.Core.UI;
using dotObjects.Core.Processing.Arguments;

namespace dotObjects.Core.Processing.Schema
{
    public class EntitySchema : SchemaBase
    {
        private List<ProcessSchema> constructors;
        private List<ProcessSchema> instanceMethods;
        [NonSerialized] private Type entityType;
        private List<ProcessSchema> staticMethods;

        public EntitySchema(Type type, EntityDomain entity)
        {
            if (type == null) throw new ArgumentNullException("type");
            EntityType = type;
            Label = CoreSection.Current.GetLocalizationString(EntityType);

            Query = GetQueryMethod();
            Constructors = GetConstructorMethods();
            StaticMethods = GetStaticMethods();
            InstanceMethods = GetInstanceMethods(entity);
        }

        public Type EntityType
        {
            get { return entityType; }
            private set { entityType = value; }
        }

        public ProcessSchema Query { get; private set; }

        public List<ProcessSchema> Constructors
        {
            get { return constructors ?? (constructors = new List<ProcessSchema>()); }
            private set { constructors = value; }
        }

        public List<ProcessSchema> StaticMethods
        {
            get { return staticMethods ?? (staticMethods = new List<ProcessSchema>()); }
            private set { staticMethods = value; }
        }

        public List<ProcessSchema> InstanceMethods
        {
            get { return instanceMethods ?? (instanceMethods = new List<ProcessSchema>()); }
            private set { instanceMethods = value; }
        }

        public override bool Visible
        {
            get
            {
                if (!Query.Visible
                    && Constructors.Count == Constructors.FindAll(ctor => !ctor.Visible).Count
                    && StaticMethods.Count == StaticMethods.FindAll(sm => !sm.Visible).Count
                    && InstanceMethods.Count == InstanceMethods.FindAll(im => !im.Visible).Count)
                    return false;
                return base.Visible;
            }
            set { base.Visible = value; }
        }


        private ProcessSchema GetQueryMethod()
        {
            string label = CoreSection.Current.GetLocalizationString(EntityType, ProcessBehavior.Query.ToString());

            FilterAttribute[] attributes = (FilterAttribute[]) EntityType.GetCustomAttributes(typeof (FilterAttribute), false);
            QueryArgument argument = new QueryArgument();
            if (attributes.Length == 1)
                argument = (QueryArgument) ProcessArgument.Parse(ProcessBehavior.Query, attributes[0].Expression);
            ProcessURI uri = new ProcessURI(CoreSection.GetTypeName(EntityType), ProcessBehavior.Query, argument);
            return CreateProcessSchema(label, uri);
        }

        private List<ProcessSchema> GetConstructorMethods()
        {
            ConstructorInfo[] ctors = EntityType.GetConstructors();

            List<ProcessSchema> elements = new List<ProcessSchema>();
            for (int i = 0; i < ctors.Length; i++)
            {
                ConstructorInfo ctor = ctors[i];

                if (ctor.GetParameters().Length > 0)
                {
                    string label = CoreSection.Current.GetLocalizationString(EntityType, ProcessBehavior.New.ToString(),
                                                                             i.ToString());

                    if (label.Equals((ProcessBehavior.New + Domain.TypeSeparator + i)))
                        label = ProcessBehavior.New.ToString();

                    ProcessURI uri = new ProcessURI(CoreSection.GetTypeName(EntityType), ProcessBehavior.New,
                                                    i.ToString());
                    elements.Add(CreateProcessSchema(label, uri));
                }
            }
            return elements;
        }

        private List<ProcessSchema> GetStaticMethods()
        {
            return GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
        }

        private List<ProcessSchema> GetInstanceMethods(EntityDomain entity)
        {
            List<ProcessSchema> methods = GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
            if (entity != null && entity.Value != null)
                methods.ForEach(ps => ps.URI.Argument.Values.Add(entity.Value.ToString()));
            return methods;
        }

        private List<ProcessSchema> GetMethods(BindingFlags flags)
        {
            MethodInfo[] methods = GetPreparedMethods(flags);
            List<ProcessSchema> elements = new List<ProcessSchema>();
            foreach (MethodInfo methodInfo in methods)
            {
                if (ContainsMethod(elements, methodInfo)) continue;
                MethodInfo[] foundMethods = FindMethodsByName(methods, methodInfo.Name);
                for (int i = 0; i < foundMethods.Length; i++)
                {
                    MethodInfo foundMethod = foundMethods[i];
                    ExecutionArgument argument = new ExecutionArgument(new List<object>(new[] {foundMethod.Name, i.ToString()}));
                    ProcessURI uri = new ProcessURI(CoreSection.GetTypeName(EntityType), ProcessBehavior.Exec, argument);
                    ProcessSchema schema = CreateProcessSchema(GetMethodLabel(foundMethod, i), uri);
                    ParameterInfo[] parameters = foundMethod.GetParameters();
                    if (parameters.Length == 1)
                    {
                        ParameterInfo parameter = parameters[0];
                        if (ComplexDomain.IsGenericEntityCollection(parameter.ParameterType) && ComplexDomain.GetEntityTypeFromCollection(parameter.ParameterType) == EntityType)
                            schema.Type = ProcessSchemaType.ForEntityCollectionOnly;
                        else if (CoreSection.Current.IsEntity(parameter.ParameterType) && parameter.ParameterType == EntityType)
                            schema.Type = ProcessSchemaType.ForEntityOnly;
                    }
                    else
                        schema.Type = ProcessSchemaType.Normal;
                    elements.Add(schema);
                }
            }
            return elements;
        }

        private MethodInfo[] GetPreparedMethods(BindingFlags flags)
        {
            MethodInfo[] methods = EntityType.GetMethods(flags);
            methods = RemovePropertyMethods(EntityType, methods);
            methods = RemoveObjectMethods(methods);
            methods = RemoveEventMethods(EntityType, methods);
            return methods;
        }

        private string GetMethodLabel(MethodInfo foundMethod, int i)
        {
            string label = CoreSection.Current.GetLocalizationString(EntityType, foundMethod.Name, i.ToString());
            if (label.Equals(foundMethod.Name + Domain.TypeSeparator + i))
                label = foundMethod.Name;
            return label;
        }

        private static ProcessSchema CreateProcessSchema(string label, ProcessURI uri)
        {
            return new ProcessSchema
                       {
                           Label = label,
                           URI = uri,
                           Visible = true
                       };
        }

        private static MethodInfo[] FindMethodsByName(MethodInfo[] methods, string methodName)
        {
            return Array.FindAll(methods, m => m.Name.Equals(methodName));
        }

        private static bool ContainsMethod(List<ProcessSchema> elements, MethodInfo methodInfo)
        {
            return elements.FindAll(s =>
                                        {
                                            ExecutionArgument arg = s.URI.Argument as ExecutionArgument;
                                            if (arg != null)
                                                return arg.Name.StartsWith(methodInfo.Name);
                                            return false;
                                        }).Count > 0;
        }

        private static MethodInfo[] RemoveObjectMethods(IEnumerable<MethodInfo> methods)
        {
            List<MethodInfo> fixedMethods = new List<MethodInfo>();
            foreach (MethodInfo method in methods)
            {
                if (IsObjectMethod(method)) continue;
                fixedMethods.Add(method);
            }
            return fixedMethods.ToArray();
        }

        private static bool IsObjectMethod(MethodInfo method)
        {
            foreach (MethodInfo m in typeof (object).GetMethods())
                if (m.Name.Equals(method.Name)) return true;
            return false;
        }

        private static MethodInfo[] RemoveEventMethods(Type type, IEnumerable<MethodInfo> methods)
        {
            List<MethodInfo> fixedMethods = new List<MethodInfo>();
            foreach (MethodInfo method in methods)
            {
                EventInfo[] events = type.GetEvents();
                if (IsEventMethod(method, events)) continue;
                fixedMethods.Add(method);
            }
            return fixedMethods.ToArray();
        }

        private static bool IsEventMethod(MethodInfo method, IEnumerable<EventInfo> events)
        {
            foreach (EventInfo e in events)
            {
                if (method.Name.Equals("add_" + e.Name)) return true;
                if (method.Name.Equals("remove_" + e.Name)) return true;
            }
            return false;
        }

        private static MethodInfo[] RemovePropertyMethods(Type type, IEnumerable<MethodInfo> methods)
        {
            List<MethodInfo> fixedMethods = new List<MethodInfo>();
            foreach (MethodInfo method in methods)
            {
                PropertyInfo[] properties = type.GetProperties();
                if (IsPropertyMethod(method, properties)) continue;
                fixedMethods.Add(method);
            }
            return fixedMethods.ToArray();
        }

        private static bool IsPropertyMethod(MethodInfo method, IEnumerable<PropertyInfo> properties)
        {
            foreach (PropertyInfo p in properties)
            {
                if (method.Name.Equals("set_" + p.Name)) return true;
                if (method.Name.Equals("get_" + p.Name)) return true;
            }
            return false;
        }
    }
}