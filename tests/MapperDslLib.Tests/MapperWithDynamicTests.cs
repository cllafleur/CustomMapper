using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MapperDslLib.Tests
{
    public class MapperWithDynamicTests
    {
        class OriginObject
        {
            public string Title { get; set; }
            public string Description { get; set; }

            public DateTime ModificationDate { get; set; }
        }
        class TargetObject
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public DateTime? ModificationDate { get; set; }

        }

        private const string CASE1 = @"Title -> Title
Description -> Description
ModificationDate -> ModificationDate
";

        [Test]
        public void Map_OriginAsDynamicTargetAsStaticType_SetRootLevel_Success()
        {
            var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
            mapper.Load();
            var handler = mapper.GetMapper<ExpandoObject, TargetObject>();
            dynamic origin = new ExpandoObject();
            origin.Title = "supertitle";
            origin.Description = "superdescription";
            origin.ModificationDate = new DateTime(2021, 07, 31);

            var target = new TargetObject();
            handler.Map(origin, target);

            Assert.AreEqual(origin.Title, target.Title);
            Assert.AreEqual(origin.ModificationDate, target.ModificationDate);
            Assert.AreEqual(origin.Description, target.Description);
        }

        [Test]
        public void Map_OriginAsDynamicTargetAsDynamicType_SetRootLevel_Success()
        {
            var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
            mapper.Load();
            var handler = mapper.GetMapper<ExpandoObject, ExpandoObject>();
            dynamic origin = new ExpandoObject();
            origin.Title = "supertitle";
            origin.Description = "superdescription";
            origin.ModificationDate = new DateTime(2021, 07, 31);

            dynamic target = new ExpandoObject();
            handler.Map(origin, target);

            Assert.AreEqual(origin.Title, target.Title);
            Assert.AreEqual(origin.ModificationDate, target.ModificationDate);
            Assert.AreEqual(origin.Description, target.Description);
        }
    }
}
