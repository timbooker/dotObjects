using System;
using dotObjects.Core.Processing;

namespace dotObjects.Core.Persistence
{
    [Serializable]
    public class OrderBy
    {
        public const string Key = "OrderBy";
        public static readonly OrderBy Empty = new OrderBy {Direction = OrderByDirection.Asc};

        public string Field { get; set; }
        public OrderByDirection Direction { get; set; }

        public static OrderBy Parse(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                int idx = 0;
                string[] values = value.Split(new[] {ProcessURI.Separator}, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length >= 2)
                {
                    if (values.Length == 3 && values[0].Equals(Key))
                        idx++;
                    return new OrderBy
                               {
                                   Field = values[idx],
                                   Direction = (OrderByDirection) Enum.Parse(typeof (OrderByDirection), values[idx + 1])
                               };
                }
            }
            return OrderBy.Empty;
        }

        public override string ToString()
        {
            return Key + ProcessURI.Separator + Field + ProcessURI.Separator + Direction;
        }
    }
}