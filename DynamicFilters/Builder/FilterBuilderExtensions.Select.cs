using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DynamicFilters.Filters;
using DynamicFilters.Filters.RangeSelect;
using DynamicFilters.Filters.Select;

namespace DynamicFilters.Builder
{
    public static partial class FilterBuilderExtensions
    {
        public static IFilterBuilder<TModel> AddRange<TModel>(
            this IFilterBuilder<TModel> filterBuilder, 
            Expression<Func<TModel,object>> paramNameSelector,
            IntRange? selectedValue, 
            int[] values)
        {
            var propInfo = GetPropertyInfo(paramNameSelector);

            filterBuilder.AddFilter(new IntRangeFilter
            {
                Name = propInfo.Name,
                Mode = GetSelectMode(propInfo),
                SelectedValue = selectedValue,
                Values = values.Select(v=>new IntRange(v)).ToArray()
            });

            return filterBuilder;
        }

        public static IFilterBuilder<TModel> AddRange<TModel>(
            this IFilterBuilder<TModel> filterBuilder,
            Expression<Func<TModel, object>> paramNameSelector,
            IntRange? selectedValue,
            IntRange[] values)
        {
            var propInfo = GetPropertyInfo(paramNameSelector);

            filterBuilder.AddFilter(new IntRangeFilter
            {
                Name = propInfo.Name,
                Mode = GetSelectMode(propInfo),
                SelectedValue = selectedValue,
                Values = values
            });

            return filterBuilder;
        }

        public static IFilterBuilder<TModel> AddSelect<TModel>(
            this IFilterBuilder<TModel> filterBuilder,
            Expression<Func<TModel, object>> paramNameSelector,
            string selectedValue,
            string[] allValues, 
            string[] availableValues = null)
        {
            return filterBuilder.AddSelect(
                paramNameSelector, 
                null, 
                null,
                selectedValue, 
                allValues, 
                availableValues);
        }

        public static IFilterBuilder<TModel> AddSelect<TModel>(
            this IFilterBuilder<TModel> filterBuilder,
            Expression<Func<TModel, object>> paramNameSelector,
            Func<IntRange, string> titleSelector,
            IntRange? selectedValue,
            IntRange[] allValues,
            IntRange[] availableValues = null)
        {
            return filterBuilder.AddSelect(
                paramNameSelector,
                titleSelector,
                v => v.To.HasValue 
                    ? $"{v.From}--{v.To.Value}" 
                    : v.From.ToString(),
                selectedValue,
                allValues,
                availableValues);
        }

        public static IFilterBuilder<TModel> AddSelect<TModel>(
            this IFilterBuilder<TModel> filterBuilder,
            Expression<Func<TModel, object>> paramNameSelector,
            int? selectedValue,
            int[] allValues,
            int[] availableValues = null)
        {
            return filterBuilder.AddSelect(
                paramNameSelector,
                null, 
                null,
                selectedValue,
                allValues,
                availableValues);
        }

        public static IFilterBuilder<TModel> AddSelect<TModel, TValue>(
            this IFilterBuilder<TModel> filterBuilder,
            Expression<Func<TModel, object>> paramNameSelector,
            Func<TValue, string> titleSelector,
            Func<TValue, string> valueSelector,
            TValue? selectedValue,
            TValue[] allValues,
            TValue[] availableValues = null,
            IEqualityComparer<TValue> comparer = null) where TValue : struct
        {
            var availableValuesSet = GetAvailableValuesSet(allValues, availableValues);

            var propInfo = GetPropertyInfo(paramNameSelector);

            var filter = new SelectFilter
            {
                Name = propInfo.Name,
                Mode = propInfo.IsCollection
                    ? SelectMode.Multi
                    : SelectMode.Single,
                Options = allValues.Select(v => new SelectFilterItem
                {
                    Title = titleSelector?.Invoke(v) ?? v.ToString(),
                    Value = valueSelector?.Invoke(v) ?? v.ToString(),
                    IsDisabled = !availableValuesSet.Contains(v),
                    IsSelected = selectedValue.HasValue && (comparer?.Equals(v, selectedValue.Value) ?? v.Equals(selectedValue.Value))
                }).ToList()
            };

            filterBuilder.AddFilter(filter);

            return filterBuilder;
        }

        public static IFilterBuilder<TModel> AddSelect<TModel,TValue>(
            this IFilterBuilder<TModel> filterBuilder,
            Expression<Func<TModel, object>> paramNameSelector,
            Func<TValue, string> titleSelector,
            Func<TValue, string> valueSelector,
            TValue selectedValue,
            TValue[] allValues,
            TValue[] availableValues = null,  
            IEqualityComparer<TValue> comparer = null) where TValue : class
        {
            var availableValuesSet = GetAvailableValuesSet(allValues, availableValues);

            var propInfo = GetPropertyInfo(paramNameSelector);

            var filter = new SelectFilter
            {
                Name = propInfo.Name,
                Mode = GetSelectMode(propInfo),
                Options = allValues.Select(v => new SelectFilterItem
                {
                    Title = titleSelector?.Invoke(v) ?? v.ToString(),
                    Value = valueSelector?.Invoke(v) ?? v.ToString(),
                    IsDisabled = !availableValuesSet.Contains(v),
                    IsSelected = selectedValue != null && (comparer?.Equals(v, selectedValue) ?? v.Equals(selectedValue))
                }).ToList()
            };

            filterBuilder.AddFilter(filter);

            return filterBuilder;
        }

        private static ModelPropertyInfo GetPropertyInfo<TModel>(Expression<Func<TModel, object>> paramNameSelector)
        {
            var memberBody = paramNameSelector.Body as MemberExpression;
            if (memberBody == null)
            {
                var ubody = (UnaryExpression) paramNameSelector.Body;
                memberBody = ubody.Operand as MemberExpression;
            }

            var propertyInfo = memberBody.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"{paramNameSelector.Type} is not property selector");
            }

            return new ModelPropertyInfo
            {
                Name = propertyInfo.Name.ToLowerInvariant(),
                IsCollection = typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) 
                               && propertyInfo.PropertyType != typeof(string)
            };
        }

        private static SelectMode GetSelectMode(ModelPropertyInfo propInfo)
        {
            return propInfo.IsCollection
                ? SelectMode.Multi
                : SelectMode.Single;
        }

        private static HashSet<TValue> GetAvailableValuesSet<TValue>(
            TValue[] allValues,
            TValue[] availableValues = null)
        {
            var hashSet = new HashSet<TValue>();

            foreach (var value in availableValues ?? allValues)
            {
                if (!hashSet.Contains(value))
                {
                    hashSet.Add(value);
                }
            }
            return hashSet;
        }

        private struct ModelPropertyInfo
        {
            public string Name { get; set; }

            public bool IsCollection { get; set; }
        }
    }
}