using System.Data.Linq;
using System.Data.Linq.Mapping;
using dotObjects.Core.UI;

namespace dotObjects.Features.Dashboard
{
    [Table(Name = "DashboardQuery")]
    public class DashboardQuery
    {
        private EntityRef<Dashboard> dashboard;
        private EntityRef<Query> query;

        public DashboardQuery()
        {
            dashboard = default(EntityRef<Dashboard>);
            query = default(EntityRef<Query>);
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        [NonVisible]
        public int Id { get; set; }

        [Column]
        [NonVisible]
        public int DashboardId { get; set; }

        [Column]
        [NonVisible]
        public int QueryId { get; set; }

        [Association(Storage = "dashboard", ThisKey = "DashboardId", OtherKey = "Id", IsForeignKey = true, DeleteOnNull = true)]
        public Dashboard Dashboard
        {
            get { return dashboard.Entity; }
            set
            {
                dashboard.Entity = value;
                if (value != null) DashboardId = value.Id;
            }
        }

        [Association(Storage = "query", ThisKey = "QueryId", OtherKey = "Id", IsForeignKey = true, DeleteOnNull = true)]
        public Query Query
        {
            get { return query.Entity; }
            set
            {
                query.Entity = value;
                if (value != null) QueryId = value.Id;
            }
        }

        public static bool operator ==(DashboardQuery left, DashboardQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DashboardQuery left, DashboardQuery right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DashboardQuery)) return false;
            return Equals((DashboardQuery) obj);
        }

        public bool Equals(DashboardQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.dashboard.Equals(dashboard) && other.query.Equals(query) && other.Id == Id &&
                   other.DashboardId == DashboardId && other.QueryId == QueryId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = dashboard.GetHashCode();
                result = (result*397) ^ query.GetHashCode();
                result = (result*397) ^ Id;
                result = (result*397) ^ DashboardId;
                result = (result*397) ^ QueryId;
                return result;
            }
        }
    }
}