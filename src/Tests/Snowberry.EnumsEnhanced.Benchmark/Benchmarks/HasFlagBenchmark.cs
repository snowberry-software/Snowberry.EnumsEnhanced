using BenchmarkDotNet.Attributes;

namespace Snowberry.EnumsEnhanced.Benchmark.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
[CategoriesColumn]
[BenchmarkCategory("HasFlag")]
public class HasFlagBenchmark
{
    [Params(TestEnum.Test4 | TestEnum.Test1 | TestEnum.Test6)]
    public TestEnum TestEnumValue { get; set; }

    [Params(TestEnum.Test1 | TestEnum.Test2 | TestEnum.Test4 | TestEnum.Test5 | TestEnum.Test6)]
    public TestEnum TestEnumFlagCheck { get; set; }

    [Benchmark(Baseline = true)]
    public bool HasFlag()
    {
        return TestEnumValue.HasFlag(TestEnumFlagCheck);
    }

    [Benchmark]
    public bool HasFlagFast()
    {
        return TestEnumValue.HasFlagFast(TestEnumFlagCheck);
    }
}