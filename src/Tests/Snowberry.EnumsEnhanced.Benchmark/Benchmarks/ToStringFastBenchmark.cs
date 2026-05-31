using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Snowberry.EnumsEnhanced.Benchmark.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
[CategoriesColumn]
[BenchmarkCategory("ToString")]
[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class ToStringFastBenchmark
{
    // Single defined value and a multi-flag combination (exercises the flag-decomposition path).
    [Params(TestEnum.Test1, TestEnum.Test4 | TestEnum.Test1 | TestEnum.Test6)]
    public TestEnum Value { get; set; }

    [Benchmark(Baseline = true)]
    public string ToString_BCL()
    {
        return Value.ToString();
    }

    [Benchmark]
    public string? ToStringFast()
    {
        return Value.ToStringFast();
    }
}
