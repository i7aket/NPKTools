using System.Reflection;
using BenchmarkDotNet.Running;

// Run all benchmarks:      dotnet run -c Release --project benchmarks/NPKTools.Benchmarks
// Run a single class:      dotnet run -c Release --project benchmarks/NPKTools.Benchmarks -- --filter *SolverBenchmarks*
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
