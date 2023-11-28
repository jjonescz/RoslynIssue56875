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
        public IEnumerable<object> Inputs()
        {
            //yield return null;
            yield return new DateTime(2023, 11, 28);
            //yield return DateTime.MaxValue;
        }

        [Benchmark(Baseline = true), ArgumentsSource(nameof(Inputs))]
        public bool Before(DateTime? d) => d == DateTime.MaxValue;

        [Benchmark, ArgumentsSource(nameof(Inputs))]
        public bool After(DateTime? d) => d.HasValue && d.GetValueOrDefault() == DateTime.MaxValue;
    }
}
