using MapperDslLib.Runtime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

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
        }

        class TargetObject
        {
            public DescriptionObject Description { get; set; }

            public DateTime ModificationDate { get; set; }

            public Dictionary<string, string> Properties { get; set; }
        }

        class ExtractRef : IExtractFunctionHandler<OriginObject>
        {
            public object GetObject(OriginObject instanceObj, params object[] args)
            {
                return $"{args[0]}__new";
            }
        }

        class AddProperty : IInsertFunctionHandler<TargetObject>
        {
            public void SetObject(TargetObject instanceObject, object value, params object[] args)
            {
                instanceObject.Properties.Add((string)args[0], (string)value);
            }
        }

        [Test]
        public void Map_ScalarProperties_Success()
        {
            var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>();
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

        [Test]
        public void Map_ScalarAndFunctionProperties_Success()
        {
            var functionProvider = new FunctionHandlerProvider();
            functionProvider.Register<IExtractFunctionHandler<OriginObject>, ExtractRef>("ExtractRef");
            functionProvider.Register<IInsertFunctionHandler<TargetObject>, AddProperty>("AddProperty");
            var mapper = new Mapper(functionProvider, new StringReader(CASE2));
            mapper.Load();
            var handler = mapper.GetMapper<OriginObject, TargetObject>();
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

    }
}
