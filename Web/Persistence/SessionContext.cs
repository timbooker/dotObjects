using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using dotObjects.Core.Persistence;
using dotObjects.Core.Persistence.Db4o;

namespace dotObjects.Web.Persistence
{
    public class SessionContext : Context
    {
        private const string ContainerKey = "Container";

        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        private static Context Context
        {
            get
            {
                if (Session[ContainerKey] == null)
                {
                    Db4oContext ctx = new Db4oContext();
                    ctx.Settings[Db4oContext.ConfigInMemory] = "true";
                    Session[ContainerKey] = ctx;
                }
                return (Context) Session[ContainerKey];
            }
        }

        public override IQueryable<T> GetSource<T>()
        {
            throw new NotImplementedException();
        }

        public override void BeginTransaction()
        {
            Context.BeginTransaction();
        }

        public override void CloseTransaction()
        {
            Context.CloseTransaction();
        }

        public override void Commit()
        {
            Context.Commit();
        }

        public override void Rollback()
        {
            Context.Rollback();
        }

        public override object Save(object o)
        {
            return Context.Save(o);
        }

        public override void SaveAll()
        {
            Context.SaveAll();
        }

        public override object Insert(object o)
        {
            return Save(o);
        }

        public override bool Delete(object o)
        {
            return Context.Delete(o);
        }

        public override object Get(object id, Type target)
        {
            return Context.Get(id, target);
        }

        public override object GetID(object instance)
        {
            return Context.GetID(instance);
        }

        public override List<object> Execute(Query query)
        {
            return Context.Execute(query);
        }

        public override void Dispose()
        {
            Context.Dispose();
            Session[ContainerKey] = null;
        }

        public override void ExecuteCommand(string command, params object[] parameters)
        {
            Context.ExecuteCommand(command, parameters);
        }

        public override IEnumerable<T> ExecuteNativeQuery<T>(string query, params object[] parameters)
        {
            return Context.ExecuteNativeQuery<T>(query, parameters);
        }
    }
}