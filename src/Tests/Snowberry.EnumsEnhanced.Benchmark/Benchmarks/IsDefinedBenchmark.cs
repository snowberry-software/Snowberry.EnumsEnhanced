using BenchmarkDotNet.Attributes;

namespace Snowberry.EnumsEnhanced.Benchmark.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
[CategoriesColumn]
[BenchmarkCategory("IsDefinedName")]
public class IsDefinedBenchmark
{
    [Params(nameof(TestEnum.Test1), nameof(TestEnum.Test6), "Invalid")]
    public string NameValue { get; set; } = "";

    [Benchmark(Baseline = true)]
    public bool IsDefined_StringName()
    {
        return Enum.IsDefined(typeof(TestEnum), NameValue);
    }

    [Benchmark]
    public bool IsDefinedFast_StringName()
    {
        return TestEnumEnhanced.IsDefinedFast(NameValue);
    }
}