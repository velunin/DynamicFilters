using System.Collections.Generic;
using DynamicFilters.Filters;

namespace DynamicFilters.Builder
{
    public interface IFilterBuilder<TModel>
    {
        IFilterBuilder<TModel> AddFilter(IFilter filter);

        IEnumerable<IFilter> Build();
    }
}