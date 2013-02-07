using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using dotObjects.Core.Configuration;
using dotObjects.Core.Processing;
using dotObjects.Core.Processing.Arguments;
using dotObjects.Core.UI;
using dotObjects.Core.Persistence;
using dotObjects.Web.Rendering.Helpers;

namespace dotObjects.Features.Dashboard
{
    [Table(Name = "Query")]
    public class Query
    {
        private readonly EntitySet<DashboardQuery> dashboardQueries;

        public Query()
        {
            dashboardQueries = new EntitySet<DashboardQuery>(dq => dq.Query = this, dq =>
            {
                dq.Dashboard = null;
                dq.Query = null;
            });
        }

        public Query(string entity, string name, [Dependency("entity")] string projection,
                     [Dependency("entity")] string criterias, [Dependency("entity")] string orderBy)
            : this()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity");

            if (string.IsNullOrEmpty(projection))
                throw new ArgumentNullException("projection");

            Entity = entity;
            Name = name;
            Projection = projection;
            Criterias = criterias;
            OrderBy = string.Format("{0}{1}{2}", Core.Persistence.OrderBy.Key, ProcessURI.Separator, orderBy);
        }

        public Query(string name, string url) : this()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            var uri = ProcessURI.Parse(UrlHelper.FromUrl(url));
            var arg = uri.Argument as QueryArgument;
            Entity = uri.Entity;
            Name = name;
            Projection = arg.Projection;
            Criterias = arg.Where.ToURIString();
            OrderBy = string.Join<OrderBy>(ProcessURI.Separator, arg.OrderBy);
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        [NonVisible]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Entity { get; set; }

        [Column(CanBeNull = true), NonVisible(ProcessBehavior.Query), Dependency("Entity")]
        public string Projection { get; set; }

        [Column(CanBeNull = true), NonVisible(ProcessBehavior.Query), Dependency("Entity")]
        public string Criterias { get; set; }

        [Column, NonVisible(ProcessBehavior.Query | ProcessBehavior.View), Dependency("Entity")]
        public string OrderBy { get; set; }

        [Association(Storage = "dashboardQueries", ThisKey = "Id", OtherKey = "QueryId")]
        internal EntitySet<DashboardQuery> DashboardQueries
        {
            get { return dashboardQueries; }
            set
            {
                dashboardQueries.Clear();
                dashboardQueries.Assign(value);
            }
        }

        [NonVisible]
        public ProcessURI GeneratedURI
        {
            get {
                return GenerateURI(Entity, Projection, Criterias, OrderBy);
            }
        }

        public RedirectProcessResponse Execute()
        {
            return new RedirectProcessResponse(GeneratedURI);
        }

        public static RedirectProcessResponse QuickExecute(string entity, [Dependency("entity")] string projection,
                     [Dependency("entity")] string criterias, [Dependency("entity")] string orderBy)
        {
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentNullException("entity");

            if (string.IsNullOrEmpty(projection))
                throw new ArgumentNullException("projection");

            return new RedirectProcessResponse(GenerateURI(entity, projection, criterias, orderBy));
        }

        private static ProcessURI GenerateURI(string entity, string projection, string criterias, string orderBy)
        {
            EntityAssembly asm = CoreSection.Current.GetAssemblyByType(entity);
            string entityName = entity.Replace(asm.RootNamespace + Domain.TypeSeparator, string.Empty);
            
            var values = new List<object>();
            
            if (!string.IsNullOrEmpty(projection))
            {
                values.Add(Core.Persistence.Query.ProjectionKey);
                values.AddRange(projection.Split(new[] {ProcessURI.Separator}, StringSplitOptions.RemoveEmptyEntries));
            }

            values.AddRange(GetCriterias(criterias));
            values.AddRange(orderBy.Split(new[] { ProcessURI.Separator }, StringSplitOptions.RemoveEmptyEntries));

            return new ProcessURI(entityName, ProcessBehavior.Query)
            {
                Argument = new QueryArgument(values)
            };
        }

        private static IEnumerable<string> GetCriterias(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] criterias = value.Split(',');
                string prefix = string.Empty;
                for (int i = 0; i < criterias.Length - 1; i++)
                {
                    prefix += "And/";
                }
                return
                    string.Concat(prefix, string.Join(ProcessURI.Separator, criterias)).Split(
                        new[] { ProcessURI.Separator }, StringSplitOptions.None);
            }
            return new string[] { };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}