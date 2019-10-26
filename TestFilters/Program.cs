using System;
using DynamicFilters.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TestFilters
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new FilterBuilder<Model>();
            var filters = builder
                .AddSelectFilter(
                    x => x.Names, 
                    "Test1", 
                    new[]
                    {
                        "Test1", 
                        "Test2", 
                        "Test3"
                    }, 
                    new[]
                    {
                        "Test1",
                        "Test3"
                    })
                .AddSelectFilter(x=>x.Ids, null, new[] {1,2,3})
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
    }
}
