using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using dotObjects.Core.UI;

namespace dotObjects.Web.Serialization.UI
{
    public class DomainJavaScriptConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
                                           JavaScriptSerializer serializer)
        {
            if (type.Equals(typeof (Domain)))
            {
                Domain domain = new Domain
                                    {
                                        ID = dictionary["ID"].ToString(),
                                        Type = Type.GetType(dictionary["Type"].ToString()),
                                        Label = dictionary["Label"].ToString(),
                                        Value = dictionary["Value"].ToString(),
                                        IsQueryable = bool.Parse(dictionary["IsQueryable"].ToString()),
                                        CanRead = bool.Parse(dictionary["CanRead"].ToString()),
                                        CanWrite = bool.Parse(dictionary["CanWrite"].ToString())
                                    };
                return domain;
            }
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new[] {typeof (Domain)}; }
        }
    }
}