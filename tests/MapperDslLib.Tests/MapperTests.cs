using MapperDslLib.Runtime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MapperDslLib.Tests
{
    public class MapperTests
    {
        private const string CASE1 = @"Title -> Description.Title
Description -> Description.Description
ModificationDate -> ModificationDate
";

        private const string CASE2 = @"
Title -> Description.Title
Description -> Description.Description
ModificationDate -> ModificationDate
ExtractRef(Description) -> AddProperty(""contractType"")
            ";

        private const string CASE3 = @"Title -> Description.Title
(Description) -> Description.Description
ModificationDate -> ModificationDate
";

        class DescriptionObject
        {
            public string Title { get; set; }
            public string Description { get; set; }

            public string Description2 { get; set; }
        }

        class OriginObject
        {
            public string Title { get; set; }
            public string Description { get; set; }

            public DateTime ModificationDate { get; set; }

            public List<DescriptionObject> Items { get; set; }
        }

        class TargetObject
        {
            public DescriptionObject Description { get; set; }

            public DateTime? ModificationDate { get; set; }

            public Dictionary<string, string> Properties { get; set; }
        }

        class ExtractRef : IExtractFunctionHandler<OriginObject>
        {
            public SourceResult GetObject(OriginObject instanceObj, Parameters parameters)
            {
                return new SourceResult()
                {
                    Result = GetResults()
                };

                IEnumerable<object> GetResults()
                {
                    foreach (var item in parameters.Values[0].Result)
                    {
                        yield return $"{item}__new";
                    }
                }
            }
        }

        class AddProperty : IInsertFunctionHandler<TargetObject>
        {
            public void SetObject(TargetObject instanceObject, SourceResult source, Parameters parameters)
            {
                instanceObject.Properties.Add((string)parameters.Values[0].Result.First(), (string)source.Result.FirstOrDefault());
            }
        }

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_ScalarProperties_Success(CompilerVersions version)
        {
            var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject()
            {
                Title = "supertitle",
                Description = "superdescription",
                ModificationDate = new DateTime(2021, 07, 31),
            };
            var target = new TargetObject() { Description = new DescriptionObject() };
            handler.Map(origin, target);

            Assert.AreEqual(origin.Title, target.Description.Title);
            Assert.AreEqual(origin.ModificationDate, target.ModificationDate);
            Assert.AreEqual(origin.Description, target.Description.Description);
        }

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_ScalarAndFunctionProperties_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<OriginObject>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddProperty>("AddProperty");
            var mapper = new Mapper(functionProvider, new StringReader(CASE2));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject()
            {
                Title = "supertitle",
                Description = "superdescription",
                ModificationDate = new DateTime(2021, 07, 31),
            };
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.AreEqual(origin.Title, target.Description.Title);
            Assert.AreEqual(origin.ModificationDate, target.ModificationDate);
            Assert.AreEqual(origin.Description, target.Description.Description);
            Assert.That(target.Properties, Contains.Key("contractType").WithValue("superdescription__new"));
        }

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_ScalarAndFunctionPropertiesWithTuple_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<OriginObject>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddProperty>("AddProperty");
            var mapper = new Mapper(functionProvider, new StringReader(CASE3));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject()
            {
                Title = "supertitle",
                Description = "superdescription",
                ModificationDate = new DateTime(2021, 07, 31),
            };
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.AreEqual(origin.Title, target.Description.Title);
            Assert.AreEqual(origin.ModificationDate, target.ModificationDate);
            Assert.AreEqual(origin.Description, target.Description.Description);
        }

        private const string CASE4 = @"(Items.Description, Items.Description2) -> AddGroupDescription(""Description"")
";

        class AddGroupDescription : IInsertFunctionHandler<TargetObject>, IInsertTupleFunctionHandler<TargetObject>
        {
            public void SetObject(TargetObject instanceObject, SourceResult source, Parameters parameters)
            {
                throw new NotSupportedException();
            }

            public void SetObject(TargetObject instanceObject, TupleSourceResult source, Parameters parameters)
            {
                foreach (var item in source.Result)
                {
                    var items = item.ToArray();
                    instanceObject.Properties.Add((string)items[0], string.Join(" ", items));
                }
            }
        }

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_TwoTupleValuesAndFunctionAddGroupDescription_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddGroupDescription>("AddGroupDescription");
            var mapper = new Mapper(functionProvider, new StringReader(CASE4));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject()
            {
                Items = new List<DescriptionObject> { new DescriptionObject() { Description = "Description", Description2 = "Description2" } }
            };
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.That(target.Properties, Contains.Key("Description").WithValue("Description Description2"));
        }

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_TwoTupleValuesMultipleAndFunctionAddGroupDescription_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddGroupDescription>("AddGroupDescription");
            var mapper = new Mapper(functionProvider, new StringReader(CASE4));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject()
            {
                Items = new List<DescriptionObject>
                {
                    new DescriptionObject() { Description = "Description", Description2 = "Description2" },
                    new DescriptionObject() { Description = "Description3", Description2 = "Description4" }
                }
            };
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.That(target.Properties, Contains.Key("Description").WithValue("Description Description2"));
            Assert.That(target.Properties, Contains.Key("Description3").WithValue("Description3 Description4"));
        }

        private const string CASE5 = @"(Items.Description, ""Text litteral"") -> AddGroupDescription(""Description"")
";

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_TwoTupleValuesWithTextLiteralAndFunctionAddGroupDescription_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddGroupDescription>("AddGroupDescription");
            var mapper = new Mapper(functionProvider, new StringReader(CASE5));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject()
            {
                Items = new List<DescriptionObject>
                {
                    new DescriptionObject() { Description = "Description" },
                    new DescriptionObject() { Description = "Description3" }
                }
            };
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.That(target.Properties, Contains.Key("Description").WithValue("Description Text litteral"));
            Assert.That(target.Properties, Contains.Key("Description3").WithValue("Description3 Text litteral"));
        }

        private const string CASE6 = @"(""const1"", ""const2"") -> AddGroupDescription(""Description"")
";

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_TwoTupleValuesLiteralAndFunctionAddGroupDescription_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddGroupDescription>("AddGroupDescription");
            var mapper = new Mapper(functionProvider, new StringReader(CASE6));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject();
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.That(target.Properties, Contains.Key("const1").WithValue("const1 const2"));
        }

        private const string CASE7 = @"(""const1"", ""const2"") -> AddGroupDescription(""Description"", (""Item1"", ""Item2""))
";

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_TupleValuesLiteralAndFunctionAddGroupDescriptionWithTupleParameter_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddGroupDescription>("AddGroupDescription");
            var mapper = new Mapper(functionProvider, new StringReader(CASE7));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject();
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.That(target.Properties, Contains.Key("const1").WithValue("const1 const2"));
        }

        private const string CASE8 = @"(constName1: ""const1"", constName1: ""const2"") -> AddGroupDescription(""Description"")
";

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_TupleNamedValuesLiteralAndFunctionAddGroupDescription_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddGroupDescription>("AddGroupDescription");
            var mapper = new Mapper(functionProvider, new StringReader(CASE8));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject();
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.That(target.Properties, Contains.Key("const1").WithValue("const1 const2"));
        }

        private const string CASE9 = @"GetRef(""test"").Description -> Description.Description
GetRef(""test"").Description2 -> Description.Description2
";
        [OutputType(typeof(DescriptionObject))]
        class GetRef : IExtractFunctionHandler<OriginObject>
        {
            public virtual SourceResult GetObject(OriginObject instanceObj, Parameters parameters)
            {
                return new SourceResult()
                {
                    Result = new[] {
                        null,
                        new DescriptionObject() { Description = "result", Description2 = "result2" },
                    },
                };
            }
        }

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_FunctionDereferencement_Success(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<OriginObject>, GetRef>(nameof(GetRef));
            var mapper = new Mapper(functionProvider, new StringReader(CASE9));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject();
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };
            handler.Map(origin, target);

            Assert.That(target.Description.Description, Is.EqualTo("result"));
            Assert.That(target.Description.Description2, Is.EqualTo("result2"));
        }

        [OutputType(typeof(DescriptionObject))]
        class GetRef2 : IExtractFunctionHandler<OriginObject>
        {
            public virtual SourceResult GetObject(OriginObject instanceObj, Parameters parameters)
            {
                return new SourceResult()
                {
                    Result = new object[] {
                        new OriginObject(),
                    },
                };
            }
        }

        [TestCase(CompilerVersions.v1)]
        [TestCase(CompilerVersions.v2)]
        public void Map_FunctionDereferencement_FailedWithException(CompilerVersions version)
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<OriginObject>, GetRef2>(nameof(GetRef));
            var mapper = new Mapper(functionProvider, new StringReader(CASE9));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>(new CompilerOptions() { Version = version });
            var origin = new OriginObject();
            var target = new TargetObject() { Description = new DescriptionObject(), Properties = new Dictionary<string, string>() };

            Assert.Throws<MapperRuntimeException>(() => handler.Map(origin, target));
        }
    }
}
