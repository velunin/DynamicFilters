using System;

namespace DynamicFilters.Filters.RangeSelect
{
    public class IntRangeFilter : FilterBase
    {
        public new string Type => "intRangeSelect";

        public IntRange? SelectedValue { get; set; }

        public IntRange[] Values { get; set; }
    }
}
