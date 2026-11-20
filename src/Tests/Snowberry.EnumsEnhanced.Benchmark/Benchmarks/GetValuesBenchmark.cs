using BenchmarkDotNet.Attributes;

namespace Snowberry.EnumsEnhanced.Benchmark.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
[CategoriesColumn]
[BenchmarkCategory("GetValues")]
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