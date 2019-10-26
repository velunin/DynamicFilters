namespace DynamicFilters.Filters
{
    public interface IFilter
    {
        string Name { get; }

        string Type { get; }

        SelectMode Mode { get;  }
    }
}