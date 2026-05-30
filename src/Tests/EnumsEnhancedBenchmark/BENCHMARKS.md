# Benchmarks

Performance comparison of the generated `*Fast` enum methods against the BCL `System.Enum` APIs,
powered by [BenchmarkDotNet](https://benchmarkdotnet.org/).

## Running

> [!IMPORTANT]
> Benchmarks **must** be run in `Release`. BenchmarkDotNet refuses to run an optimized build from a
> Debug configuration and the numbers would be meaningless anyway.

The entry point uses `BenchmarkSwitcher`, so the benchmark(s) to run are selected from CLI args —
no source edits required.

```bash
# Run everything
dotnet run -c Release --project Tests/EnumsEnhancedBenchmark -- --filter *

# Run a single class
dotnet run -c Release --project Tests/EnumsEnhancedBenchmark -- --filter *HasFlagBenchmark*

# Run one or more categories
dotnet run -c Release --project Tests/EnumsEnhancedBenchmark -- --anyCategories ToString ParseName

# List the available benchmarks without running them
dotnet run -c Release --project Tests/EnumsEnhancedBenchmark -- --list flat
```

## Layout

- One class per surface area under [`Benchmarks/`](Benchmarks), each tagged with a
  `[BenchmarkCategory]` (shown via `[CategoriesColumn]`) so they can be filtered with
  `--anyCategories` / `--allCategories`.
- Each class declares a single `[Benchmark(Baseline = true)]` (the `System.Enum` method) so the
  `Ratio` column compares the `*Fast` variant against the BCL baseline.
- `[MemoryDiagnoser]` is enabled everywhere, so allocations are reported alongside timings.
- Jobs target `net48`, `net9.0`, and `net10.0`.

| Category | Class | Compares |
| --- | --- | --- |
| `HasFlag` | `HasFlagBenchmark` | `Enum.HasFlag` vs `HasFlagFast` |
| `GetName` | `GetNameBenchmark` | `Enum.GetName` vs `GetNameFast` |
| `GetNames` | `GetNamesBenchmark` | `Enum.GetNames` vs `GetNamesFast` |
| `GetValues` | `GetValuesBenchmark` | `Enum.GetValues` vs `GetValuesFast` |
| `IsDefinedName` | `IsDefinedBenchmark` | `Enum.IsDefined(string)` vs `IsDefinedFast(string)` |
| `IsDefinedValue` | `IsDefinedValueBenchmark` | `Enum.IsDefined(value)` vs `IsDefinedFast(value)` |
| `ParseName` | `ParseNameBenchmark` | `Enum.Parse(name)` vs `ParseFast(name)` |
| `ParseValue` | `ParseValueBenchmark` | `Enum.Parse(value)` vs `ParseFast(value)` |
| `ToString` | `ToStringFastBenchmark` | `Enum.ToString` vs `ToStringFast` |
