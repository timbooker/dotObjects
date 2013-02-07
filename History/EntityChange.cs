using System;
using System.Data.Linq.Mapping;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace dotObjects.History
{
    [Table(Name = "EntityChange")]
    public class EntityChange
    {
        #region Constructors

        public EntityChange()
        {
        }

        public EntityChange(DateTime time, string uri, object entityId, object entity, string userId = null)
        {
            Time = time;
            URI = uri;
            EntityType = entity.GetType().FullName;
            EntityId = entityId.ToString();
            EntityValue = Serialize(entity);
            UserId = userId;
        }

        #endregion

        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]
        public DateTime Time { get; set; }

        [Column(CanBeNull = true)]
        public string UserId { get; set; }

        [Column]
        public string URI { get; set; }
        
        [Column]
        public string EntityType { get; set; }

        [Column]
        public string EntityId { get; set; }

        [Column]
        public string EntityValue { get; set; }

        #endregion

        #region Private Methods

        private static string Serialize(object entity)
        {
            if (entity != null)
            {
                var serializer = new JavaScriptSerializer { RecursionLimit = 1 };
                var builder = new StringBuilder();
                serializer.Serialize(entity, builder);
            }
            return string.Empty;
        }

        private static object Deserialize(string serialized, Type type)
        {
            var serializer = new JavaScriptSerializer { RecursionLimit = 1 };
            return serializer.Deserialize(serialized, type);
        }

        #endregion

    }
}
