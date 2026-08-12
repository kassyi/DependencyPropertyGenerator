using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Kassyi.Generators.DependencyProperty.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance));

        BenchmarkRunner.Run<GeneratorBenchmark>(config, args);
    }
}

