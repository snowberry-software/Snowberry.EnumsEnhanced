# Benchmarks

Performance comparison of the generated `*Fast` enum methods against the BCL `System.Enum` APIs,
powered by [BenchmarkDotNet](https://benchmarkdotnet.org/).

## Running

> [!IMPORTANT]
> Benchmarks **must** be run in `Release`. BenchmarkDotNet refuses to run an optimized build from a
> Debug configuration and the numbers would be meaningless anyway.

The entry point uses `BenchmarkSwitcher`, so the benchmarks and the runtimes are selected from CLI args; no source
edits are required. Pick the runtimes with `--runtimes`. Without it, BenchmarkDotNet only runs the runtime of the host
(`-f`).

The host must be the lowest common API denominator of the runtimes it runs, because BenchmarkDotNet generates every
job from the host build. Some benchmarks only exist on .NET (`#if NETCOREAPP`, the generic `Enum` APIs), so run
.NET Framework 4.8 from a `net48` host and .NET 10 and .NET 11 from a .NET host:

```bash
# Run everything on .NET 10 and .NET 11
dotnet run -c Release -f net11.0 --project Tests/Snowberry.EnumsEnhanced.Benchmark -- --filter * --runtimes net10.0 net11.0

# Run everything on .NET Framework 4.8 (Windows only)
dotnet run -c Release -f net48 --project Tests/Snowberry.EnumsEnhanced.Benchmark -- --filter * --runtimes net48

# Run a single class
dotnet run -c Release -f net11.0 --project Tests/Snowberry.EnumsEnhanced.Benchmark -- --filter *HasFlagBenchmark* --runtimes net10.0 net11.0

# Run one or more categories
dotnet run -c Release -f net11.0 --project Tests/Snowberry.EnumsEnhanced.Benchmark -- --anyCategories ToString ParseName --runtimes net11.0

# List the available benchmarks without running them
dotnet run -c Release -f net11.0 --project Tests/Snowberry.EnumsEnhanced.Benchmark -- --list flat
```

## Layout

- One class per surface area under [`Benchmarks/`](Benchmarks), each tagged with a
  `[BenchmarkCategory]` (shown via `[CategoriesColumn]`) so they can be filtered with
  `--anyCategories` / `--allCategories`.
- Each class declares a single `[Benchmark(Baseline = true)]` (the `System.Enum` method) so the
  `Ratio` column compares the `*Fast` variant against the BCL baseline.
- `[MemoryDiagnoser]` is enabled everywhere, so allocations are reported alongside timings.
- No class declares jobs; the runtimes come from `--runtimes`, so a new runtime only needs a new target framework in the
  project and a new argument.

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
