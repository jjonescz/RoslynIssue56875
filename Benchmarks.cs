using System;
using System.Collections.Generic;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;

namespace RoslynIssue56875
{
    [HideColumns("Error", "StdDev", "Median", "RatioSD")]
    [DisassemblyDiagnoser(maxDepth: 0, printSource: true, exportHtml: true, exportCombinedDisassemblyReport: true, exportDiff: true)]
    public class Benchmarks
    {
        private static readonly Random random = new();

        public IEnumerable<object> Inputs()
        {
            yield return random.Next(3) switch
            {
                0 => null,
                1 => new DateTime(2023, 11, 28),
                2 => DateTime.MaxValue,
            };
        }

        [Benchmark(Baseline = true), ArgumentsSource(nameof(Inputs))]
        public bool Before(DateTime? d)
        {
            var x = d;
            var y = DateTime.MaxValue;
            return x.HasValue == true ? (x.HasValue ? x.GetValueOrDefault() == y : true) : false;
        }

        [Benchmark, ArgumentsSource(nameof(Inputs))]
        public bool After(DateTime? d)
        {
            var x = d;
            var y = DateTime.MaxValue;
            return x.HasValue == true ? x.GetValueOrDefault() == y : false;
        }
    }
}
