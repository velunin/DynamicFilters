using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace DynamicFilters.Filters
{
    internal class FilterTypeExtractor
    {
        private static readonly ConcurrentDictionary<Type, string> _typeMapCache = new ConcurrentDictionary<Type, string>();

        private static readonly Regex filterRegex = new Regex("^(?<filterName>)Filter$", RegexOptions.Compiled);
        
        public static string ExtractFilterType(Type type)
        {
            if (!_typeMapCache.TryGetValue(type, out var typeName))
            {
                typeName = type.Name;
                var match = filterRegex.Match(typeName);

                if (match.Success)
                {
                    typeName = match.Groups[0].Value;
                }

                typeName = char.ToLowerInvariant(typeName[0]) + typeName.Substring(1);

                _typeMapCache.TryAdd(type, typeName);
            }

            return typeName;
        }
    }
}