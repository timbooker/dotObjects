using System;
using System.Collections.Generic;
using dotObjects.Core.Persistence;

namespace dotObjects.Core.Processing.Arguments
{
    [Serializable]
    public class QueryArgument : ProcessArgument
    {
        private string toString;

        public QueryArgument()
            : base(new List<object>())
        {
        }

        public QueryArgument(List<object> values) :
            base(values)
        {
        }

        public bool Live { get; private set; }

        public string Name { get; set; }

        public int Skip { get; private set; }

        public int Take { get; set; }

        public Criteria Where { get; set; }

        public string Projection { get; set; }

        public OrderBy[] OrderBy { get; set; }

        protected override void ExtractValues(List<object> values)
        {
            List<object> arguments = new List<object>(values);
            Live = ExtractIfContains(ref arguments, Query.LiveKey);
            Name = ExtractField(ref arguments, Query.NameKey);


            var skip = ExtractField(ref arguments, Query.SkipKey);
            Skip = !string.IsNullOrEmpty(skip) ? int.Parse(skip) : -1;
            var take = ExtractField(ref arguments, Query.TakeKey);
            Take = !string.IsNullOrEmpty(take) ? int.Parse(take) : -1;

            Projection = ExtractProjection(ref arguments);
            OrderBy = ExtractOrderBy(ref arguments);
            
            Where = (Criteria) CreateCriteria(arguments);
        }

        private static OrderBy[] ExtractOrderBy(ref List<object> values)
        {
            List<OrderBy> orders = new List<OrderBy>();
            var key = dotObjects.Core.Persistence.OrderBy.Key;
            while (values.Contains(key) && values.IndexOf(key) <= values.Count - 3)
            {
                var keyIndex = values.IndexOf(key);
                var ordering = new OrderBy
                                   {
                                       Field = values[keyIndex + 1].ToString(),
                                       Direction =
                                           (OrderByDirection)
                                           Enum.Parse(typeof (OrderByDirection), values[keyIndex + 2].ToString())
                                   };
                values.RemoveRange(keyIndex, 3);
                orders.Add(ordering);
            }
            return orders.ToArray();
        }

        private static bool ExtractIfContains(ref List<object> arguments, string key)
        {
            if (arguments.Contains(key))
            {
                arguments.Remove(key);
                return true;
            }
            return false;
        }

        private static string ExtractField(ref List<object> values, string key)
        {
            int idx = values.IndexOf(key);
            if (idx >= 0)
            {
                values.RemoveAt(idx);
                var value = values[idx];
                values.RemoveAt(idx);
                return value.ToString();
            }
            return string.Empty;
        }

        private static string ExtractProjection(ref List<object> values)
        {
            int idx = values.IndexOf(Query.ProjectionKey);
            if (idx >= 0)
            {
                values.RemoveAt(idx);
                var val = values[idx];
                values.RemoveAt(idx);
                return val.ToString();
            }
            return string.Empty;
        }

        private static object CreateCriteria(List<object> args)
        {
            if (args.Count > 0)
            {
                object arg = args[0];
                args.RemoveAt(0);

                switch (arg.ToString())
                {
                    case "And":
                        return new And((Criteria) CreateCriteria(args), (Criteria) CreateCriteria(args));
                    case "Between":
                        return new Between((string) CreateCriteria(args), CreateCriteria(args), CreateCriteria(args));
                    case "Contains":
                        return new Contains((string) CreateCriteria(args), CreateCriteria(args));
                    case "Equals":
                        return new Equals((string) CreateCriteria(args), CreateCriteria(args));
                    case "NotEquals":
                        return new NotEquals((string) CreateCriteria(args), CreateCriteria(args));
                    case "GreaterThan":
                        return new GreaterThan((string) CreateCriteria(args), CreateCriteria(args));
                    case "GreaterThanOrEqual":
                        return new GreaterThanOrEqual((string) CreateCriteria(args), CreateCriteria(args));
                    case "IsNull":
                        return new IsNull((string) CreateCriteria(args));
                    case "IsNotNull":
                        return new IsNotNull((string) CreateCriteria(args));
                    case "LessThan":
                        return new LessThan((string) CreateCriteria(args), CreateCriteria(args));
                    case "LessThanOrEqual":
                        return new LessThanOrEqual((string) CreateCriteria(args), CreateCriteria(args));
                    case "Or":
                        return new Or((Criteria) CreateCriteria(args), (Criteria) CreateCriteria(args));
                    case "In":
                        return new In((string) CreateCriteria(args), CreateCriteria(args));
                }
                return arg;
            }
            return null;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(toString))
                toString = string.Join(ProcessURI.Separator, Values.ToArray());
            return toString;
        }

        private List<object> CleanStandardKeys()
        {
            var values = new List<object>(Values);
            ExtractField(ref values, Query.SkipKey);
            ExtractField(ref values, Query.TakeKey);
            ExtractProjection(ref values);
            ExtractOrderBy(ref values);
            return values;
        }

        public string[] GetProjectionElements()
        {
            if (!string.IsNullOrEmpty(Projection))
                return Projection.Split(',');
            return new string[] {};
        }
    }
}