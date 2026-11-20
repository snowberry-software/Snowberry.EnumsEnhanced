using BenchmarkDotNet.Attributes;

namespace Snowberry.EnumsEnhanced.Benchmark.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
[CategoriesColumn]
[BenchmarkCategory("GetNames")]
public class GetNamesBenchmark
{
    [Benchmark(Baseline = true)]
    public string[] GetNames()
    {
        return Enum.GetNames(typeof(TestEnum));
    }

#if NETCOREAPP
    [Benchmark]
    public string[] GetNames_Generic()
    {
        return Enum.GetNames<TestEnum>();
    }
#endif

    [Benchmark]
    public string[] GetNamesFast()
    {
        return TestEnumEnhanced.GetNamesFast();
    }
}