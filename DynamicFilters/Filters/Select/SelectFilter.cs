using System.Collections.Generic;

namespace DynamicFilters.Filters.Select
{
    public class SelectFilter : FilterBase
    {
        public List<SelectFilterItem> Options { get; set; }
    }
}