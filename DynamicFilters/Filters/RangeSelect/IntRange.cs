namespace DynamicFilters.Filters.RangeSelect
{
    public struct IntRange
    {
        public IntRange(int from, int? to = default)
        {
            From = from;
            To = to;
        }

        public int From { get; }

        public int? To { get; }
    }
}