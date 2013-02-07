using System;
using System.Collections.Generic;
using System.IO;
using dotObjects.Core.Serialization;

namespace dotObjects.Core.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FileDomain : FieldDomain
    {
        [NonSerialized]
        private object value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="type"></param>
        /// <param name="parent"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public FileDomain(string label, Type type, Domain parent, object value, string id)
            : base(label, type, parent, value, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [NonSerializable]
        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public override void Fill(Dictionary<string, object> data)
        {
            if (data.ContainsKey(UniqueID) && data[UniqueID] != null)
            {
                Stream stream = data[UniqueID] as Stream;
                if (stream != null && stream.Length > 0)
                    Value = stream;
            }
        }
    }
}