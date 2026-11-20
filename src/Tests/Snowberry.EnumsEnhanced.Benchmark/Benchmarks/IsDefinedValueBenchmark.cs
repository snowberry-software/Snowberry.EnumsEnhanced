using BenchmarkDotNet.Attributes;

namespace Snowberry.EnumsEnhanced.Benchmark.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
[CategoriesColumn]
[BenchmarkCategory("IsDefinedValue")]
public class IsDefinedValueBenchmark
{
    [Params(TestEnum.Test1, TestEnum.Test7)]
    public TestEnum EnumValue { get; set; }

    [Benchmark(Baseline = true)]
    public bool IsDefined_Value()
    {
        return Enum.IsDefined(typeof(TestEnum), EnumValue);
    }

#if NETCOREAPP
    [Benchmark]
    public bool IsDefined_Value_Generic()
    {
        return Enum.IsDefined(EnumValue);
    }
#endif

    [Benchmark]
    public bool IsDefinedFast_Value()
    {
        return TestEnumEnhanced.IsDefinedFast(EnumValue);
    }
}