using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Snowberry.EnumsEnhanced.Benchmark.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
[CategoriesColumn]
[BenchmarkCategory("GetValues")]
[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class GetValuesBenchmark
{
    [Benchmark(Baseline = true)]
    public Array GetValues()
    {
        return Enum.GetValues(typeof(TestEnum));
    }

#if NETCOREAPP
    [Benchmark]
    public Array GetValues_Generic()
    {
        return Enum.GetValues<TestEnum>();
    }
#endif

    [Benchmark]
    public Array GetValuesFast()
    {
        return TestEnumEnhanced.GetValuesFast();
    }
}
