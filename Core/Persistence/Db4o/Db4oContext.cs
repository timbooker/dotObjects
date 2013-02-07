using System;
using System.Collections.Generic;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using dotObjects.Core.Configuration;
using System.Drawing;

namespace dotObjects.Core.Persistence.Db4o
{
    /// <summary>
    /// DB4O implementation of the dotObjects Persistence Context.
    /// </summary>
    public class Db4oContext : Context
    {
        private const string ConfigCallbacks = "Callbacks";
        private const string ConfigFile = "File";
        private const string ConfigHost = "Host";
        public const string ConfigInMemory = "InMemory";
        private const string ConfigLock = "Lock";
        private const string ConfigPassword = "Password";
        private const string ConfigPort = "Port";
        private const string ConfigServer = "Server";
        private const string ConfigUsername = "Username";

        private const int MaxDepth = 9999;

        private IConfiguration configuration;
        private IObjectContainer container;

        private IObjectContainer Container
        {
            get
            {
                if (container == null)
                {
                    if (Settings.ContainsKey(ConfigCallbacks))
                        Configuration.Callbacks(bool.Parse(Settings[ConfigCallbacks]));
                    if (Settings.ContainsKey(ConfigLock))
                        Configuration.LockDatabaseFile(bool.Parse(Settings[ConfigLock]));

                    Configuration.ActivationDepth(MaxDepth);
                    Configuration.UpdateDepth(MaxDepth);

                    Configuration.ObjectClass(typeof (Image)).Translate(new TSerializable());
                    Configuration.ObjectClass(typeof (Bitmap)).Translate(new TSerializable());

                    if (Settings.ContainsKey(ConfigFile))
                        container = Db4oFactory.OpenFile(Configuration, Settings[ConfigFile]);
                    else if (Settings.ContainsKey(ConfigServer))
                    {
                        container = Db4oFactory.OpenClient(Configuration, Settings[ConfigHost],
                                                           int.Parse(Settings[ConfigPort]), Settings[ConfigUsername],
                                                           Settings[ConfigPassword]);
                    }
                    else if (Settings.ContainsKey(ConfigInMemory))
                        container = ExtDb4oFactory.OpenMemoryFile(Configuration, new MemoryFile());
                    else
                        throw new Exception(string.Format("The {0} cannot open the specified container.",
                                                          typeof (Db4oContext).FullName));
                }
                return container;
            }
        }

        private IConfiguration Configuration
        {
            get { return configuration ?? (configuration = Db4oFactory.Configure()); }
        }

        /// <summary>
        /// Begins a Context's transaction.
        /// </summary>
        public override void BeginTransaction()
        {
        }

        /// <summary>
        /// Close the current Context's transaction.
        /// </summary>
        public override void CloseTransaction()
        {
        }

        /// <summary>
        /// Commits the realized operations in transaction.
        /// </summary>
        public override void Commit()
        {
            Container.Commit();
        }

        /// <summary>
        /// Rollback the realized operations in transaction.
        /// </summary>
        public override void Rollback()
        {
            Container.Rollback();
        }

        /// <summary>
        /// Save (insert or update) the object in Context.
        /// </summary>
        /// <param name="o">A storable object's instance.</param>
        /// <returns>Returns the objectid of the saved instance</returns>
        /// <exception cref="Exception">if some exception occurs during the process.</exception>
        public override object Save(object o)
        {
            Container.Store(o);
            object id = Container.Ext().GetID(o).ToString();
            return id;
        }

        public override void SaveAll()
        {
            
        }

        /// <summary>
        /// Save (insert) the object in Context.
        /// </summary>
        /// <param name="o">A storable object's instance.</param>
        /// <returns>Returns the objectid of the saved instance</returns>
        /// <exception cref="Exception">if some exception occurs during the process.</exception>
        public override object Insert(object o)
        {
            return Save(o);
        }

        /// <summary>
        /// Delete the object from Context.
        /// </summary>
        /// <param name="o">A storable object's instance.</param>
        /// <returns>Return True if success and False if fail.</returns>
        public override bool Delete(object o)
        {
            try
            {
                Container.Delete(o);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the object by its identifier.
        /// </summary>
        /// <param name="id">The unique object identifier.</param>
        /// <param name="target">The target type.</param>
        /// <returns>A storable object's instance.</returns>
        public override object Get(object id, Type target)
        {
            try
            {
                object instance = Container.Ext().GetByID(long.Parse(id.ToString()));
                Container.Activate(instance, Configuration.ActivationDepth());
                return instance;
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Gets the object's instance in current context.
        /// </summary>
        /// <param name="instance">The entity instance.</param>
        /// <returns>Return the object's instance in current context.</returns>
        public override object GetID(object instance)
        {
            return Container.Ext().GetID(instance);
        }

        /// <summary>
        /// Gets all storable objects that match with 'query' parameter.
        /// </summary>
        /// <param name="query">A Context Query's instance.</param>
        /// <returns>A dictionary whose TKey is the object´s instance and TValue is the object instance.</returns>
        public override List<object> Execute(Query query)
        {
            IQuery soda = Container.Query();
            soda.Constrain(query.Target);
            CreateConstraint(soda, query, query.Where);
            return ToList(soda.Execute());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Container.Close();
            Container.Dispose();
        }

        private static List<object> ToList(IObjectSet result)
        {
            List<object> items = new List<object>();
            while (result.HasNext())
                items.Add(result.Next());
            return items;
        }

        private IConstraint CreateConstraint(IQuery soda, Query query, Criteria c)
        {
            if (c is Equals) return CreateEqualsConstraint(soda, query, c);
            if (c is Contains) return CreateContainsConstraint(soda, c);
            if (c is And) return CreateAndConstraint(soda, query, c);
            if (c is Or) return CreateOrConstraint(soda, query, c);
            if (c is IsNull) return CreateIsNullConstraint(soda, c);
            if (c is Between) return CreateBetweenConstraint(soda, query, c);
            if (c is LessThan) return CreateLessThanConstraint(soda, c);
            if (c is LessThanOrEqual) return CreateLessThanOrEqualConstraint(soda, query, c);
            if (c is GreaterThan) return CreateGreatherThanConstraint(soda, c);
            if (c is GreaterThanOrEqual) return CreateGreaterThanOrEqualConstraint(soda, query, c);
            return null;
        }

        private IConstraint CreateGreaterThanOrEqualConstraint(IQuery soda, Query query, Criteria c)
        {
            GreaterThanOrEqual greaterEqual = c as GreaterThanOrEqual;
            return CreateConstraint(soda, query,
                                    new GreaterThan(greaterEqual.FirstElement.ToString(), greaterEqual.SecondElement))
                .Or(CreateConstraint(soda, query,
                                     new Equals(greaterEqual.FirstElement.ToString(), greaterEqual.SecondElement)));
        }

        private static IConstraint CreateGreatherThanConstraint(IQuery soda, Criteria c)
        {
            GreaterThan greater = c as GreaterThan;
            return soda.Descend(greater.FirstElement.ToString()).Constrain(greater.SecondElement).Greater();
        }

        private IConstraint CreateLessThanOrEqualConstraint(IQuery soda, Query query, Criteria c)
        {
            LessThanOrEqual lessEqual = c as LessThanOrEqual;
            return CreateConstraint(soda, query,
                                    new LessThan(lessEqual.FirstElement.ToString(), lessEqual.SecondElement))
                .Or(CreateConstraint(soda, query, new Equals(lessEqual.FirstElement.ToString(), lessEqual.SecondElement)));
        }

        private static IConstraint CreateLessThanConstraint(IQuery soda, Criteria c)
        {
            LessThan less = c as LessThan;
            return soda.Descend(less.FirstElement.ToString()).Constrain(less.SecondElement).Smaller();
        }

        private IConstraint CreateBetweenConstraint(IQuery soda, Query query, Criteria c)
        {
            Between between = c as Between;
            return CreateConstraint(soda, query,
                                    new GreaterThanOrEqual(between.FirstElement.ToString(), between.SecondElement))
                .And(CreateConstraint(soda, query,
                                      new LessThanOrEqual(between.FirstElement.ToString(), between.ThirdElement)));
        }

        private static IConstraint CreateIsNullConstraint(IQuery soda, Criteria c)
        {
            IsNull isnull = c as IsNull;
            return soda.Descend(isnull.FirstElement.ToString()).Constrain(null).Equal();
        }

        private IConstraint CreateOrConstraint(IQuery soda, Query query, Criteria c)
        {
            Or or = c as Or;
            return CreateConstraint(soda, query, (Criteria) or.FirstElement)
                .Or(CreateConstraint(soda, query, (Criteria) or.SecondElement));
        }

        private IConstraint CreateAndConstraint(IQuery soda, Query query, Criteria c)
        {
            And and = c as And;
            return CreateConstraint(soda, query, (Criteria) and.FirstElement)
                .And(CreateConstraint(soda, query, (Criteria) and.SecondElement));
        }

        private static IConstraint CreateContainsConstraint(IQuery soda, Criteria c)
        {
            Contains cont = c as Contains;
            return soda.Descend(cont.FirstElement.ToString()).Constrain(cont.SecondElement).Contains();
        }

        private IConstraint CreateEqualsConstraint(IQuery soda, Query query, Criteria c)
        {
            Equals eq = c as Equals;
            FieldInfo field = query.Target.GetField(eq.FirstElement.ToString(),
                                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null && CoreSection.Current.IsEntity(field.FieldType))
            {
                eq.SecondElement = Get(eq.SecondElement.ToString(), field.FieldType);
                return soda.Descend(eq.FirstElement.ToString()).Constrain(eq.SecondElement).ByExample();
            }
            return soda.Descend(eq.FirstElement.ToString()).Constrain(eq.SecondElement).Equal();
        }

        public override System.Linq.IQueryable<T> GetSource<T>()
        {
            throw new NotImplementedException();
        }

        public override void ExecuteCommand(string command, params object[] parameters)
        {
            
        }

        public override IEnumerable<T> ExecuteNativeQuery<T>(string query, params object[] parameters)
        {
            return new List<T>();
        }
    }
}