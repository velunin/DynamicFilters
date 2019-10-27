using System;
using DynamicFilters.Builder;
using DynamicFilters.Filters.RangeSelect;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TestFilters
{
    class Program
    {
        static void Main(string[] args)
        {
            var names = new string[]
            {
                "Test1",
                "Test2",
                "Test3"
            };

            string RatingTitleSelector(IntRange r) => r.To.HasValue 
                ? $"от {r.From} до {r.To.Value}" 
                : $"от {r.From}";

            var ranges = new[]
            {
                new IntRange(1,5),
                new IntRange(5,10),
                new IntRange(10)   
            };

            var builder = new FilterBuilder<Model>();
            var filters = builder
                .AddSelect(
                    x => x.Names, 
                    null,
                    names)
                .AddSelect(x=>x.Ids, null, new[] {1,2,3})
                .AddSelect(x=>x.RatingFrom, x => "Высокий рейтинг", null, null, new[] {7})
                .AddSelect(x=>x.RatingFrom, RatingTitleSelector, new IntRange(5,10), ranges)
                .Build();

            Console.WriteLine(JsonConvert.SerializeObject(
                filters,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                }));
        }
    }

    public class Model
    {
        public string[] Names { get; set; }

        public int[] Ids { get; set; }

        public int RatingFrom { get; set; }
    }
}
