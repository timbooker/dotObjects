using System;

namespace dotObjects.Core.Processing.Schema
{
    [Serializable]
    public abstract class SchemaBase
    {
        private string label;
        private bool? visible;

        public string Label
        {
            get { return label; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
                label = value;
            }
        }

        public virtual bool Visible
        {
            get { return !visible.HasValue || visible.Value; }
            set { visible = value; }
        }
    }
}