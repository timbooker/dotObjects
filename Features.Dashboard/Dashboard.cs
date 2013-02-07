using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using dotObjects.Core.UI;

namespace dotObjects.Features.Dashboard
{
    [Table(Name = "Dashboard")]
    public class Dashboard
    {
        private readonly EntitySet<DashboardQuery> dashboardQueries;

        public Dashboard()
        {
            dashboardQueries = new EntitySet<DashboardQuery>(dq => dq.Dashboard = this, dq =>
            {
                dq.Dashboard = null;
                dq.Query = null;
            });
        }

        ///TODO: Fix the Dashboard constructor validation.
        public Dashboard(string name, List<Query> queries)
            : this()
        {
            if (string.IsNullOrEmpty(name) || queries == null || queries.Count == 0)
                throw new ArgumentException();

            Name = name;
            queries.ForEach(q => DashboardQueries.Add(new DashboardQuery {Dashboard = this, Query = q}));
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        [NonVisible]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Association(Storage = "dashboardQueries", ThisKey = "Id", OtherKey = "DashboardId")]
        internal EntitySet<DashboardQuery> DashboardQueries
        {
            get { return dashboardQueries; }
            set
            {
                dashboardQueries.Clear();
                dashboardQueries.Assign(value);
            }
        }

        public List<Query> Queries
        {
            get
            {
                List<Query> queries = new List<Query>();
                foreach (var dashboardQuery in DashboardQueries)
                {
                    queries.Add(dashboardQuery.Query);
                }
                return queries;
            }
            set
            {
                if (value != null)
                {
                    DashboardQueries.Clear();
                    value.ForEach(q => DashboardQueries.Add(new DashboardQuery {Dashboard = this, Query = q}));
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}