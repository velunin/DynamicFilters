using System.Collections.Generic;
using DynamicFilters.Filters;

namespace DynamicFilters.Builder
{
    public class FilterBuilder<TModel> : IFilterBuilder<TModel>
    {
        private readonly List<IFilter> _filters = new List<IFilter>();

        public IFilterBuilder<TModel> AddFilter(IFilter filter)
        {
            _filters.Add(filter);

            return this;
        }

        public IEnumerable<IFilter> Build()
        {
            return _filters;
        }
    }
}