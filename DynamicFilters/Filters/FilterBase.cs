namespace DynamicFilters.Filters
{
    public abstract class FilterBase : IFilter
    {
        public string Type => FilterTypeExtractor.ExtractFilterType(GetType());

        public string Name { get; set; }

        public SelectMode Mode { get; set; }
    }
}