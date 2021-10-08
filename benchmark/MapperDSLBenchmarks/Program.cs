using BenchmarkDotNet.Running;
using System;
using System.Reflection;

namespace MapperDSLBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(Assembly.GetExecutingAssembly());
            //new MapperBenchmark().ParallelBench();
        }
    }
}
