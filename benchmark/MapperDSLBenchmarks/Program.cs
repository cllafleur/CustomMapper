using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Reflection;

namespace MapperDSLBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MapperBenchmark>(ManualConfig
                    .Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
            //new MapperBenchmark().ParallelBench();
        }
    }

    class Config : ManualConfig
    {
        public Config()
        {
            this.WithOptions(ConfigOptions.DisableOptimizationsValidator);
        }
    }
}
