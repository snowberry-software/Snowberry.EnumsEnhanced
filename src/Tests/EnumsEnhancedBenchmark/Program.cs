using BenchmarkDotNet.Running;

// Runs any/all benchmarks selected from CLI args, e.g.:
//   dotnet run -c Release -- --filter *
//   dotnet run -c Release -- --filter *HasFlag*
//   dotnet run -c Release -- --anyCategories ToString Parsing
//   dotnet run -c Release -- --list flat
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
