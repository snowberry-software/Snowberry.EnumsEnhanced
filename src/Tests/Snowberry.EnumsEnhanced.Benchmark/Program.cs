using BenchmarkDotNet.Running;

// Runs the benchmarks selected from CLI args; pick the runtimes with --runtimes, e.g.:
//   dotnet run -c Release -f net11.0 -- --filter * --runtimes net10.0 net11.0
//   dotnet run -c Release -f net48 -- --filter * --runtimes net48
//   dotnet run -c Release -f net11.0 -- --filter *HasFlag* --runtimes net11.0
//   dotnet run -c Release -f net11.0 -- --list flat
//
// Without --runtimes, BenchmarkDotNet only runs the runtime of the host (-f). The host must be the lowest common API
// denominator of the runtimes it runs: some benchmarks only exist on .NET (#if NETCOREAPP, the generic Enum APIs), so
// .NET Framework 4.8 is run from a net48 host and .NET 10 and .NET 11 from a .NET host.
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);