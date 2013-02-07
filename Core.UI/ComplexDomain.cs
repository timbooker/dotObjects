using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using dotObjects.Core.Configuration;
using dotObjects.Core.Persistence;
using dotObjects.Core.Processing;
using System.ComponentModel;
using dotObjects.Core.Processing.Arguments;
using dotObjects.Security.Configuration;
using dotObjects.Security.Configuration.Auth;

namespace dotObjects.Core.UI
{
    [Serializable]
    public class ComplexDomain : Domain
    {
        [NonSerialized]
        private object instance;
        private ProcessURI uri;

        protected ComplexDomain(Type type, ProcessURI processURI, object instance)
            : base(type)
        {
            Type = type;
            URI = processURI;
            Instance = instance;
            CreateLabel();
        }

        public ComplexDomain(Type type, ProcessURI processURI, object instance, string title)
            : this(type, processURI, instance)
        {
            Title = title;
        }

        protected ProcessURI URI
        {
            get { return uri; }
            private set { uri = value; }
        }


        public object Instance
        {
            get
            {
                if (instance == null && HasValue)
                    instance = ContextFactory.GetContext(Type).Get(Value, Type);
                return instance;
            }
            set { instance = value; }
        }

        public override object ObjectValue
        {
            get { return Instance; }
        }

        public bool IsInteractiveWith(ProcessBehavior behavior)
        {
            var attrs = Type.GetCustomAttributes(typeof(NonInteractiveAttribute), false);
            if (attrs.Length > 0)
            {
                var interactivity = (NonInteractiveAttribute)attrs[0];
                return (interactivity.Behavior & behavior) != behavior;
            }
            return true;
        }

        private void CreateLabel()
        {
            CoreSection config = CoreSection.Current;
            switch (URI.Behavior)
            {
                case ProcessBehavior.New:
                    const string newLabel = "New";
                    List<string> keys = new List<string> { newLabel };
                    URI.Argument.Values.ForEach(v => keys.Add(v.ToString()));
                    Label = config.GetLocalizationString(Type, keys.ToArray());
                    if (Label.Equals(string.Join(TypeSeparator, keys.ToArray())))
                        Label = newLabel;
                    break;
                case ProcessBehavior.View:
                    Label = config.GetLocalizationString(Type, "View");
                    break;
                case ProcessBehavior.Edit:
                    Label = config.GetLocalizationString(Type, "Edit");
                    break;
                case ProcessBehavior.Exec:
                    List<string> args = new List<string>();
                    URI.Argument.Values.ForEach(v => args.Add(v.ToString()));
                    Label = config.GetLocalizationString(Type, args.ToArray());

                    if (Label.Equals(string.Join(TypeSeparator, args)))
                        Label = URI.Argument.Values[0].ToString();
                    break;
            }
        }

        protected override void CreateChildDomains()
        {
            if (Domains.Count == 0)
            {
                switch (URI.Behavior)
                {
                    case ProcessBehavior.New:
                        IndexedArgument indexedArg = URI.Argument as IndexedArgument;
                        if (indexedArg != null)
                        {
                            ConstructorInfo[] ctors = Type.GetConstructors();
                            CreateMethodDomains(ctors, indexedArg.Index);
                        }
                        break;
                    case ProcessBehavior.Exec:
                        ExecutionArgument execArg = URI.Argument as ExecutionArgument;
                        if (execArg != null)
                        {
                            Value = execArg.Id ?? Value;
                            MethodInfo[] methods = Array.FindAll(Type.GetMethods(), m => m.Name.Equals(execArg.Name));
                            CreateMethodDomains(methods, execArg.Index);
                        }
                        break;
                    default:
                        Domains.AddRange(GetPropertyDomains(Type, Instance));
                        break;
                }
            }
        }


        private void CreateMethodDomains(MethodBase[] methods, int index)
        {
            if (index < methods.Length)
            {
                AuthorizationURI auri = SecuritySection.Current.Authorization.URIs.Find(URI);
                foreach (ParameterInfo p in methods[index].GetParameters())
                {
                    var name = p.Name;
                    var type = p.ParameterType;
                    var label = CoreSection.Current.GetLocalizationString(Type, name);
                    var dependency = GetMemberAttribute<DependencyAttribute>(p);
                    var format = GetMemberAttribute<FormatAttribute>(p);
                    var filter = GetMemberAttribute<FilterAttribute>(p);
                    Domains.Add(CreateDomain(name, type, label, null, dependency, format, filter, auri, true, true, true));
                }
            }
        }

        private IEnumerable<Domain> GetPropertyDomains(Type type, object domainInstance)
        {
            var domains = new DomainCollection();

            if (type != null)
            {
                domains.AddRange(GetPropertyDomains(type.BaseType, domainInstance));
                AuthorizationURI auri = SecuritySection.Current.Authorization.URIs.Find(URI);
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
                foreach (PropertyInfo p in type.GetProperties(flags))
                {
                    var name = p.Name;
                    var propertyType = p.PropertyType;
                    var label = CoreSection.Current.GetLocalizationString(Type, name);
                    var dependency = GetMemberAttribute<DependencyAttribute>(p);
                    var format = GetMemberAttribute<FormatAttribute>(p);
                    var filter = GetMemberAttribute<FilterAttribute>(p);
                    var canRead = p.CanRead && p.GetGetMethod() != null;
                    var isQueryable = GetMemberAttribute<NonQueryableAttribute>(p) == null;

                    var canWrite = p.CanWrite && p.GetSetMethod() != null;
                    if (IsGenericEntityCollection(p.PropertyType))
                        canWrite = canWrite && !p.PropertyType.IsSubclassOf(typeof(ReadOnlyCollection<>));

                    var nonVisible = GetMemberAttribute<NonVisibleAttribute>(p);
                    var visible = nonVisible == null || (nonVisible.Behavior & URI.Behavior) != URI.Behavior;
                    var instance = domainInstance != null ? p.GetValue(domainInstance, null) : null;
                    var domain = CreateDomain(name, propertyType, label, instance, dependency, format, filter, auri, canWrite, canRead, visible);
                    
                    domain.IsQueryable = isQueryable;
                    domains.Add(domain);
                }
                ReorderDomains(ref domains);
            }

            return domains;
        }

        private void ReorderDomains(ref DomainCollection domains)
        {
            var argument = uri.Argument as QueryArgument;

            if (argument != null)
            {
                string[] elements = argument.GetProjectionElements();

                if (elements.Length > 0)
                    domains.ForEach(f => f.Visible = false);

                for (int i = 0; i < elements.Length; i++)
                {
                    var domain = domains.Find(f => f.ID.Equals(elements[i]));
                    if (domain != null)
                    {
                        domain.Visible = true;
                        domains.RemoveAt(domains.IndexOf(domain));
                        domains.Insert(i, domain);
                    }
                }
            }
        }

        private Domain CreateDomain(string name, Type type, string label, object instance, DependencyAttribute dependency, FormatAttribute format, FilterAttribute filter, AuthorizationURI auri, bool canWrite, bool canRead, bool visible)
        {
            var domain = CreateDomain(type, name, label, instance);

            domain.CanWrite = canWrite;
            domain.CanRead = canRead;
            domain.Visible = visible;

            domain.DependsOf = dependency != null ? dependency.ID : string.Empty;
            domain.Format = format;

            filter = filter ?? new FilterAttribute(string.Empty);
            if (auri != null)
            {
                AuthorizationRule rule = AuthorizationManager.GetRule(auri);
                if (rule != null)
                {
                    AuthorizationRuleFilter authFilter = rule.Filters.Find(domain.ID);
                    filter = authFilter != null
                                ? new FilterAttribute(authFilter.Expression)
                                : filter;
                }
            }
            filter.Domain = domain;
            domain.Filter = filter;
            return domain;
        }

        

        private Domain CreateDomain(Type type, string id, string label, object instance)
        {
            if (CoreSection.Current.IsEntity(type))
            {
                var objectInstanceId = (instance != null && type.IsInstanceOfType(instance)) ? ContextFactory.GetContext(type).GetID(instance) : null;

                return new EntityDomain(type, new ProcessURI(type.FullName, URI.Behavior, URI.Argument), objectInstanceId, instance) { Label = label, ID = id };
            }

            if (IsGenericEntityCollection(type))
            {
                var args = type.GetGenericArguments();

                return new EntityCollectionDomain(args[0], type, new ProcessURI(args[0].FullName, URI.Behavior, URI.Argument), instance, this) { Label = label, ID = id };
            }

            if (typeof(Stream).Equals(type))
                return new FileDomain(label, type, this, instance, id);

            if (typeof(Image).Equals(type))
                return new ImageDomain(label, type, this, instance, id);

            return new FieldDomain(label, type, this, instance, id);
        }

        public static bool IsGenericEntityCollection(Type type)
        {
            if (type.IsGenericType)
            {
                Type def = type.GetGenericTypeDefinition();
                if (def.GetInterface(typeof(ICollection<>).Name) != null)
                {
                    Type[] args = type.GetGenericArguments();
                    if (args.Length > 0 && CoreSection.Current.IsEntity(args[0]))
                        return true;
                }
            }
            return false;
        }

        public static Type GetEntityTypeFromCollection(Type type)
        {
            if (IsGenericEntityCollection(type) && type.GetGenericArguments().Length > 0)
                return type.GetGenericArguments()[0];
            return null;
        }

        public static object CreateEntityInstance(EntityDomain entity, int constructorIndex)
        {
            MethodBase method = GetMethodByIndex(entity.Type.GetConstructors(), constructorIndex);
            return ((ConstructorInfo)method).Invoke(GetMethodParameters(method, entity));
        }

        public static object[] GetMethodParameters(MethodBase method, EntityDomain entity)
        {
            List<object> args = new List<object>();
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0 && entity != null && parameters.Length == entity.Domains.Count)
            {
                foreach (Domain param in entity.Domains)
                {
                    ConvertValue(param);
                    args.Add(param.ObjectValue);
                }
            }
            return args.ToArray();
        }

        private static MethodBase GetMethodByIndex(IList<MethodBase> methods, int methodIndex)
        {
            if (methodIndex >= methods.Count)
                throw new IndexOutOfRangeException();
            return methods[methodIndex];
        }

        public static MethodBase GetMethodByName(IEnumerable<MethodBase> methods, string name, int index)
        {
            List<MethodBase> found = new List<MethodBase>();
            foreach (MethodBase method in methods)
            {
                if (method.Name.Equals(name))
                    found.Add(method);
            }
            return GetMethodByIndex(found, index);
        }

        public static void ConvertValue(Domain domain)
        {
            if (domain is EntityDomain && CoreSection.Current.IsEntity(domain.Type))
            {
                EntityDomain entity = domain as EntityDomain;
                if (IsValidDomain(entity) && !(domain is EntityCollectionDomain) && domain.HasValue)
                {
                    entity.Instance = ContextFactory.GetContext(domain.Type).Get(domain.Value, domain.Type);
                }
                else
                    domain.Value = null;
            }
            else if (domain.Value == null || string.IsNullOrEmpty(domain.Value.ToString()))
            {
                domain.Value = null;
            }
            else
            {
                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(domain.Type);
                    if (converter != null && !typeof(Stream).IsInstanceOfType(domain.Value)) 
                        domain.Value = converter.ConvertFrom(domain.Value);
                }
                catch (Exception)
                {
                }
            }
        }

        protected static void SynchronizeEntity(EntityDomain entity)
        {
            foreach (var item in (from p in entity.Type.GetProperties()
                                  join d in entity.Domains.Where(d => d.CanWrite && IsValidDomain(d))
                                  on p.Name equals d.ID
                                  select new { p, d }))
            {
                ConvertValue(item.d);
                if (item.d is EntityDomain)
                    Synchronize((EntityDomain)item.d);
                item.p.SetValue(entity.Instance, item.d.ObjectValue, null);
            }
        }

        protected static void Synchronize(EntityDomain param)
        {
            foreach (var item in (from p in param.Type.GetProperties()
                                  join d in param.Domains.Where(d => d.CanWrite && d.Visible && IsValidDomain(d))
                                  on p.Name equals d.ID
                                  select new { p, d }))
            {
                ConvertValue(item.d);
                item.p.SetValue(param.Instance, item.d.ObjectValue, null);
            }
        }

        private static bool IsValidDomain(Domain param)
        {
            return param is EntityCollectionDomain || param.Value != null &&
                   !string.IsNullOrEmpty(param.Value.ToString()) &&
                   !param.Value.ToString().Equals("0");
        }

        private static T GetMemberAttribute<T>(ICustomAttributeProvider member)
        {
            object[] attrs = member.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0)
                return (T)attrs[0];
            return default(T);
        }
    }
}