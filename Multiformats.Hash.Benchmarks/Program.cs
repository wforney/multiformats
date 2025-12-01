using BenchmarkDotNet.Running;
using Multiformats.Hash.Benchmarks;

new BenchmarkSwitcher([typeof(SumBenchmarks)]).Run(args);
