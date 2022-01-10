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
using MapperDslUI.Models.Target.Properties;

namespace MapperDSLBenchmarks
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
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
        private void DoStaticMap(VacancyDetailRead origin, JobAd target)
        {
            target.Reference = origin.Reference;
            target.Properties.Add("OfferReference", new SingleProperty() { Items = new List<SinglePropertyItem>() { new SinglePropertyItem { Label = origin.Reference } } });
            target.JobAdDetails.Title = origin.JobDescription.JobTitle;
            target.JobAdDetails.MissionDescription = origin.JobDescription.Description1;
            target.JobAdDetails.ProfileDescription = origin.JobDescription.Description2;
            target.Properties.Add("Organisation", new SingleProperty { Items = new List<SinglePropertyItem> { new SinglePropertyItem { Id = origin.Organisation.Id, Label = origin.Organisation.Label } } });
            target.Properties.Add("Country", new SingleProperty { Items = new List<SinglePropertyItem> { new SinglePropertyItem { Id = origin.JobDescription.Country.Id, Label = origin.JobDescription.Country.Label } } });
            target.Location.Address = origin.JobDescription.JobDescriptionCustomFields.CustomCodeTable1.Label;
            target.Location.Coordinates.Latitude = 42.90;
            target.Location.Coordinates.Longitude = -44.967;
            target.Properties.Add("ContractType", new SingleProperty { Items = new List<SinglePropertyItem> { new SinglePropertyItem { Id = origin.JobDescription.ContractType.Id, Label = origin.JobDescription.ContractType.Label } } });
            target.Properties.Add("InternalInformations", new SingleProperty { Items = new List<SinglePropertyItem> { new SinglePropertyItem { Label = origin.Criteria.CriteriaCustomFields.LongText1 } } });
            target.Properties.Add("CreationDate", new SingleProperty { Items = new List<SinglePropertyItem> { new SinglePropertyItem { Label = origin.CreationDate.ToString() } } });
        }

        private VacancyDetailRead origin;

        public MapperBenchmark()
        {
            origin = ModelBuilder.GetNewVacancyDetailRead();
        }

        [Params(100/*, 1000, 10000*/)]
        public long IterationNumber { get; set; } = 100000000;

        [Benchmark]
        public void BuildOnceV1()
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<JobAd>, AddProperty>("AddProperty");
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, GenerateId>("GenerateId");
            var mapper = new Mapper(functionProvider, new StringReader(DEFAULT_DEFINITION));
            var (success, errors) = mapper.Load();
            var mapperHandler = mapper.GetMapper<VacancyDetailRead, JobAd>(CompileOption.v1);
            for (int i = 0; i < IterationNumber; i++)
            {
                var target = ModelBuilder.GetNewJobAd();
                mapperHandler.Map(origin, target);
            }
        }

        [Benchmark]
        public void BuildOnceV2()
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<JobAd>, AddProperty>("AddProperty");
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, GenerateId>("GenerateId");
            var mapper = new Mapper(functionProvider, new StringReader(DEFAULT_DEFINITION));
            var (success, errors) = mapper.Load();
            var mapperHandler = mapper.GetMapper<VacancyDetailRead, JobAd>(CompileOption.v2);
            for (int i = 0; i < IterationNumber; i++)
            {
                var target = ModelBuilder.GetNewJobAd();
                mapperHandler.Map(origin, target);
            }
        }

        [Benchmark]
        public void StaticMap()
        {
            for (int i = 0; i < IterationNumber; i++)
            {
                var target = ModelBuilder.GetNewJobAd();
                DoStaticMap(origin, target);
            }
        }

        [Benchmark]
        public void BuildEachTimeV1()
        {
            for (int i = 0; i < IterationNumber; i++)
            {
                BlockToTest(CompileOption.v1);
            }
        }

        [Benchmark]
        public void BuildEachTimeV2()
        {
            for (int i = 0; i < IterationNumber; i++)
            {
                BlockToTest(CompileOption.v2);
            }
        }

        //[Benchmark]
        public void BuildEachTimeWithCache()
        {
            Settings.EnableReflectionCaching = true;
            for (int i = 0; i < IterationNumber; i++)
            {
                BlockToTest(CompileOption.v1);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void BlockToTest(CompileOption option)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<JobAd>, AddProperty>("AddProperty");
            functionProvider.Register<IExtractFunctionHandler<VacancyDetailRead>, GenerateId>("GenerateId");
            var mapper = new Mapper(functionProvider, new StringReader(DEFAULT_DEFINITION));
            var (success, errors) = mapper.Load();
            var mapperHandler = mapper.GetMapper<VacancyDetailRead, JobAd>(option);
            var target = ModelBuilder.GetNewJobAd();
            mapperHandler.Map(origin, target);
        }

        //[Benchmark]
        public void ParallelBench()
        {
            Parallel.For(0L, IterationNumber, new ParallelOptions() { MaxDegreeOfParallelism = 20 }, (i) =>
             {
                 BlockToTest(CompileOption.v1);
             });
        }
    }
}
