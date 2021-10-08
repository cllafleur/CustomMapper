using MapperDslLib.Runtime;
using MapperDslLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapperDslUI.Models.Origin;
using MapperDSLBenchmarks.Models;
using MapperDslUI.Models.Target;
using MapperDslUI.Models;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Runtime.CompilerServices;

namespace MapperDSLBenchmarks
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    public class MapperBenchmark
    {
        private const string DEFAULT_DEFINITION = @"
GenerateId(Reference) -> Reference
Reference -> AddProperty(""OfferReference"")
JobDescription.JobTitle -> JobAdDetails.Title
JobDescription.Description1 -> JobAdDetails.MissionDescription
JobDescription.Description2 -> JobAdDetails.ProfileDescription
Organisation -> AddProperty(""Organisation"")
JobDescription.Country -> AddProperty(""Country"")
JobDescription.JobDescriptionCustomFields.CustomCodeTable1.Label -> Location.Address
""42.90"" -> Location.Coordinates.Longitude
""-44.967"" -> Location.Coordinates.Latitude
JobDescription.ContractType -> AddProperty(""ContractType"")
Criteria.CriteriaCustomFields.LongText1 -> AddProperty(""InternalInformations"") # c'est un commentaire !
ExtractRef(CreationDate) -> AddProperty(""CreationDate"")
";
        private VacancyDetailRead origin;

        public MapperBenchmark()
        {
            origin = ModelBuilder.GetNewVacancyDetailRead();
        }

        [Params(100, 1000, 10000)]
        public long IterationNumber { get; set; } = 100000000;

        [Benchmark]
        public void FirstBench()
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<JobAd>, AddProperty>("AddProperty");
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, GenerateId>("GenerateId");
            var mapper = new Mapper(functionProvider, new StringReader(DEFAULT_DEFINITION));
            var (success, errors) = mapper.Load();
            var mapperHandler = mapper.GetMapper<VacancyDetailRead, JobAd>();
            for (int i = 0; i < IterationNumber; i++)
            {
                var target = ModelBuilder.GetNewJobAd();
                mapperHandler.Map(origin, target);
            }
        }

        [Benchmark]
        public void SecondBench()
        {
            for (int i = 0; i < IterationNumber; i++)
            {
                BlockToTest();
            }
        }

        [Benchmark]
        public void SecondBenchWithCache()
        {
            Settings.EnableReflectionCaching = true;
            for (int i = 0; i < IterationNumber; i++)
            {
                BlockToTest();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void BlockToTest()
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<JobAd>, AddProperty>("AddProperty");
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, GenerateId>("GenerateId");
            var mapper = new Mapper(functionProvider, new StringReader(DEFAULT_DEFINITION));
            var (success, errors) = mapper.Load();
            var mapperHandler = mapper.GetMapper<VacancyDetailRead, JobAd>();
            var target = ModelBuilder.GetNewJobAd();
            mapperHandler.Map(origin, target);
        }

        //[Benchmark]
        public void ParallelBench()
        {
            Parallel.For(0L, IterationNumber, new ParallelOptions() { MaxDegreeOfParallelism = 20 }, (i) =>
             {
                 BlockToTest();
             });
        }
    }
}
