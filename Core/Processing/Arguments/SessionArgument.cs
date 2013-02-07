using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotObjects.Core.Processing.Arguments
{
    public class SessionArgument : ProcessArgument
    {
        public const string SetKey = "Set";
        public const string RemoveKey = "Remove";
        private Dictionary<string, object> sets;
        private List<object> removes;

        public SessionArgument(List<object> values)
            : base(values)
        {
        }

        public Dictionary<string, object> Sets
        {
            get
            {
                if (sets == null)
                    sets = new Dictionary<string, object>();
                return sets;
            }
        }

        public List<object> Removes
        {
            get
            {
                if (removes == null)
                    removes = new List<object>();
                return removes;
            }
        }

        protected override void ExtractValues(List<object> values)
        {
            List<object> arguments = new List<object>(values);
            ExtractSets(ref arguments);
            ExtractRemoves(ref arguments);
        }

        private void ExtractRemoves(ref List<object> arguments)
        {
            for (int i = 0; i < arguments.Count; i = i + 2)
            {
                if (arguments[i].Equals(SessionArgument.RemoveKey))
                    Removes.Add(arguments[i + 1]);
            }
        }

        private void ExtractSets(ref List<object> arguments)
        {
            for (int i = 0; i < arguments.Count; i = i + 3)
            {
                if (arguments[i].Equals(SessionArgument.SetKey))
                    Sets.Add(arguments[i + 1].ToString(), arguments[i + 2]);
            }
        }
    }
}
