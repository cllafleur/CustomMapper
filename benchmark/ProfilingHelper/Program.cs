// See https://aka.ms/new-console-template for more information


using MapperDSLBenchmarks;

var ben = new MapperBenchmark();
ben.IterationNumber = 1000000;
ben.ParseOnceCompileEachTimeV2();

Console.WriteLine("Done");
Console.ReadLine();