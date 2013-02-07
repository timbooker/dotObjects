using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using dotObjects.Session;

namespace dotObjects.Core.Persistence.LinqToSQL
{
    /// <summary>
    /// 
    /// </summary>
    public class LinqToSqlContext : Context
    {
        private const string MappingUrl = "MappingUrl";
        private const string ConnectionString = "ConnectionString";
        private const string ConnectionStringName = "ConnectionStringName";

        private MappingSource source;
        private DataContext dataContext;
        private bool isCommited;

        private MappingSource Source
        {
            get
            {
                if (source == null)
                {
                    if (Settings.ContainsKey(MappingUrl))
                        source = XmlMappingSource.FromUrl(Settings[MappingUrl]);
                    else
                        source = new AttributeMappingSource();
                }
                return source;
            }
            set { source = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataContext DataContext
        {
            get
            {
                if (dataContext == null)
                {
                    var connection = GetConnectionString(null);
                    dataContext = new DataContext(connection, Source) { CommandTimeout = 120 };
                }

                if (dataContext.Connection.State == ConnectionState.Closed)
                    dataContext.Connection.Open();
                return dataContext;
            }
        }

        private string GetConnectionString(string connection)
        {
            if (Settings.ContainsKey(ConnectionString))
                connection = Settings[ConnectionString];
            if (Settings.ContainsKey(ConnectionStringName))
                connection = ConfigurationManager.ConnectionStrings[Settings[ConnectionStringName]].ConnectionString;
            if (string.IsNullOrEmpty(connection))
                throw new ConfigurationErrorsException(
                    "Missing ConnectionString or ConnectionStringName setting for dotObjects's Linq to Sql context.");
            return connection;
        }

        public override void Dispose()
        {
            if (DataContext != null) DataContext.Dispose();
            Source = null;
            IsReleased = true;
        }

        public override void BeginTransaction()
        {
            if (DataContext.Transaction == null)
                DataContext.Transaction = DataContext.Connection.BeginTransaction();
        }

        public override void CloseTransaction()
        {
            DataContext.Transaction = null;
        }

        public override void Commit()
        {
            if (DataContext.Transaction != null)
            {
                DataContext.Transaction.Commit();
                isCommited = true;
            }
        }

        public override void Rollback()
        {
            if (DataContext.Transaction != null && !isCommited)
                DataContext.Transaction.Rollback();
        }

        public override object Save(object instance)
        {
            ITable table = GetTable(instance.GetType());
            return SubmitChanges(instance, table);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SaveAll()
        {
            DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        public override object Insert(object instance)
        {
            ITable table = GetTable(instance.GetType());
            table.InsertOnSubmit(instance);
            return SubmitChanges(instance, table);
        }

        private object SubmitChanges(object instance, ITable table)
        {
            DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
            MetaType type = DataContext.Mapping.GetMetaType(instance.GetType());
            if (type.IdentityMembers.Count > 0)
            {
                foreach (ModifiedMemberInfo member in table.GetModifiedMembers(instance))
                    if (type.IdentityMembers[0].Name.Equals(member.Member.Name))
                        return member.CurrentValue;
            }
            return null;
        }

        public override bool Delete(object o)
        {
            GetTable(o.GetType()).DeleteOnSubmit(o);
            DataContext.SubmitChanges(ConflictMode.FailOnFirstConflict);
            return true;
        }

        public override object Get(object id, Type target)
        {
            Query query = CreateQuery(target);
            foreach (var identity in DataContext.Mapping.GetMetaType(target).IdentityMembers)
            {
                Criteria condition = new Equals(identity.Name, id);
                query.Where = query.Where == null ? condition : query.Where.Or(condition);
            }
            return Execute(query).SingleOrDefault();
        }

        public override object GetID(object instance)
        {
            var type = DataContext.Mapping.GetMetaType(instance.GetType());
            if (type.IdentityMembers.Count > 0)
            {
                var identity = type.IdentityMembers[0];
                var property = instance.GetType().GetProperty(identity.Name);
                if (property != null)
                    return property.GetValue(instance, identity.Type.IsArray ? new object[] { 0 } : null);
            }
            return null;
        }

        public override List<object> Execute(Query query)
        {
            var table = GetTable(query.Target);
            var arguments = new List<object>();
            var expression = CreateExpression(query.Target, query.Where, ref arguments);
            var result = query.Where != null ? table.Where(expression, arguments.ToArray()) : table;

            if (query.Skip >= 0)
                result = result.Skip(query.Skip);

            if (query.Take > 0)
                result = result.Take(query.Take);

            if (query.OrderBy != null && query.OrderBy.Length > 0)
            {
                for (int i = query.OrderBy.Length - 1; i >= 0; i--)
                {
                    var orderby = query.OrderBy[i];
                    result = result.OrderBy(orderby.Field + " " + orderby.Direction.ToString().ToLowerInvariant());
                }
            }

            DataContext.Refresh(RefreshMode.OverwriteCurrentValues, result);

            var items = new List<object>();
            foreach (object item in result)
            {
                if (query.Target == item.GetType())
                    items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public ITable GetTable(Type target)
        {
            ITable table = null;
            while (table == null && target != null)
            {
                try
                {
                    table = DataContext.GetTable(target);
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException)
                        target = target.BaseType;
                }
            }
            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="criteria"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public string CreateExpression(Type target, Criteria criteria, ref List<object> arguments)
        {
            if (criteria is Equals)
            {
                var elementType = GetElementType(target, criteria.FirstElement);
                var value = Encapsulate(target, criteria);
                if (elementType == typeof(DateTime))
                    return string.Format("{0}.Date == DateTime.Parse({1}).Date", criteria.FirstElement, value);
                if (elementType.IsEnum)
                    return string.Format("({0}.Equals({1}) || {0} == {1})", criteria.FirstElement, value);
                return FormatExpression(criteria.FirstElement, " == ", value);
            }

            if (criteria is NotEquals)
            {
                var value = Encapsulate(target, criteria);
                return GetElementType(target, criteria.FirstElement) == typeof(DateTime) 
                    ? string.Format("{0}.Date != DateTime.Parse({1}).Date", criteria.FirstElement, value) 
                    : FormatExpression(criteria.FirstElement, " != ", value);
            }

            if (criteria is Contains)
            {
                object value = Encapsulate(target, criteria);
                return string.Format("{0}.Contains({1})", criteria.FirstElement, value);
            }

            if (criteria is And)
            {
                return FormatExpression(CreateExpression(target, (Criteria)criteria.FirstElement, ref arguments), " && ",
                                        CreateExpression(target, (Criteria)criteria.SecondElement, ref arguments));
            }

            if (criteria is Or)
            {
                return FormatExpression(CreateExpression(target, (Criteria)criteria.FirstElement, ref arguments), " || ",
                                        CreateExpression(target, (Criteria)criteria.SecondElement, ref arguments));
            }

            if (criteria is In)
            {
                var value = Encapsulate(target, criteria);
                arguments.Add(value);
                return string.Format("{0}.Contains(@{1})", criteria.FirstElement, arguments.Count - 1);
            }

            if (criteria is IsNull)
            {
                return FormatExpression(criteria.FirstElement, " == ", "null");
            }

            if (criteria is IsNotNull)
            {
                return FormatExpression(criteria.FirstElement, " != ", "null");
            }

            if (criteria is Between)
            {
                var between = criteria as Between;
                var min = FormatExpression(between.FirstElement, " >= ", between.SecondElement);
                var max = FormatExpression(between.FirstElement, " <= ", between.ThirdElement);
                return FormatExpression(min, " && ", max);
            }

            if (criteria is LessThan)
            {
                return FormatExpression(criteria.FirstElement, " < ", Convert(target, criteria.FirstElement, criteria.SecondElement));
            }

            if (criteria is LessThanOrEqual)
            {
                return FormatExpression(criteria.FirstElement, " <= ", Convert(target, criteria.FirstElement, criteria.SecondElement));
            }

            if (criteria is GreaterThan)
            {
                return FormatExpression(criteria.FirstElement, " > ", Convert(target, criteria.FirstElement, criteria.SecondElement));
            }

            if (criteria is GreaterThanOrEqual)
            {
                return FormatExpression(criteria.FirstElement, " >= ", Convert(target, criteria.FirstElement, criteria.SecondElement));
            }

            //TODO: Implement In and NotIn Criterias
            return null;
        }

        private object Convert(Type target, object field, object value)
        {
            var type = GetElementType(target, field);
            try
            {
                return type == typeof(Decimal)
                           ? string.Format("Decimal.Parse(\"{0}\")", value)
                           : type.IsGenericType ? value : System.Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                return value;
            }
        }

        private static object Encapsulate(Type target, Criteria criteria)
        {
            return ShouldBeEncapsulated(target, criteria.FirstElement)
                       ? GetElementType(target, criteria.FirstElement) == typeof(char)
                             ? string.Format("'{0}'", criteria.SecondElement)
                             : GetElementType(target, criteria.FirstElement) == typeof(DateTime)
                                ? criteria.SecondElement.ToString().Replace("{day}", string.Format("\"{0}\"", SessionManager.Session["day"]))
                                : string.Format("\"{0}\"", criteria.SecondElement)
                       : criteria.SecondElement;
        }

        private static bool ShouldBeEncapsulated(Type target, object firstElement)
        {
            Type propertyType = GetElementType(target, firstElement);
            return ShouldBeEncapsulated(propertyType);
        }

        private static bool ShouldBeEncapsulated(Type propertyType)
        {
            return propertyType == typeof (string) || propertyType == typeof (char) ||
                   propertyType == typeof (Guid) ||
                   propertyType.IsSubclassOf(typeof (string)) || propertyType.IsSubclassOf(typeof (char)) ||
                   propertyType.IsSubclassOf(typeof (Guid)) ||
                   propertyType.IsSubclassOf(typeof (Enum)) ||
                   propertyType == typeof (DateTime);
        }

        private static Type GetElementType(Type target, object element)
        {
            return GetPropertyType(target, SplitField(element.ToString()));
        }

        private static Type GetPropertyType(Type target, List<string> properties)
        {
            Type type = target.GetProperty(properties[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).PropertyType;
            properties.RemoveAt(0);
            return (properties.Count > 0) ? GetPropertyType(type, properties) : type;
        }

        private static string FormatExpression(object first, string @operator, object second)
        {
            return string.Format("({0}{1}{2})", first, @operator, second);
        }

        public override IQueryable<T> GetSource<T>()
        {
            return GetTable(typeof(T)) as IQueryable<T>;
        }

        public override void ExecuteCommand(string command, params object[] parameters)
        {
            DataContext.ExecuteCommand(command, parameters);
        }

        public override IEnumerable<T> ExecuteNativeQuery<T>(string query, params object[] parameters)
        {
            return DataContext.ExecuteQuery<T>(query, parameters);
        }
    }
}