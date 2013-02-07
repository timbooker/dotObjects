using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace dotObjects.Core.UI
{
    [Serializable]
    public class ImageDomain : FieldDomain
    {
        public ImageDomain(string label, Type type, Domain parent, object value, string id)
            : base(label, type, parent, value, id)
        {
        }

        public override void Fill(Dictionary<string, object> data)
        {
            if (data.ContainsKey(UniqueID) && data[UniqueID] != null)
            {
                Stream stream = data[UniqueID] as Stream;
                if (stream != null && stream.Length > 0)
                    Value = Image.FromStream(stream);
            }
        }
    }
}