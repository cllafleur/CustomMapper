using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Csharp;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MapperDslLib.Tests.Runtime.Csharp
{
    public class GetInstanceVisitorGeneratorTests
    {
        public class Test1
        {
            public string Text { get; set; }

            public List<string> List { get; set; }

            public Test1[] List2 { get; set; }
        }

        [Test]
        public void GetInstanceVisitorGeneratorTests_ToGetScalarValue()
        {
            var obj = new Test1() { Text = "currentText" };

            IGetInstanceVisitor<Test1> visitor = new GetInstanceVisitorGenerator<Test1>("Text");
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.FirstOrDefault(), Is.EqualTo("currentText"));
        }

        [Test]
        public void GetInstanceVisitorGeneratorTests_ToGetListOfSimpleValue()
        {
            var obj = new Test1() { List = new List<string>() { "value1", "value2" } };

            IGetInstanceVisitor<Test1> visitor = new GetInstanceVisitorGenerator<Test1>("List");
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(obj.List));
        }

        [Test]
        public void GetInstanceVisitorGeneratorTests_ToGetListOfComplexValue()
        {
            var obj = new Test1() { List2 = new Test1[] { new Test1 { Text = "value1" }, new Test1 { Text = "value2" } } };

            IGetInstanceVisitor<Test1> visitor = new GetInstanceVisitorGenerator<Test1>("List2.Text");
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2" }));
        }


        [Test]
        public void GetInstanceVisitorGeneratorTests_ToGetListOfComplexAndSimpleValue()
        {
            var obj = new Test1()
            {
                List2 = new Test1[] {
                new Test1 { List = new List<string> { "value1", "value2" } },
                new Test1 { List = new List<string>{ "value3", "value4" } } }
            };

            IGetInstanceVisitor<Test1> visitor = new GetInstanceVisitorGenerator<Test1>("List2.List");
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2", "value3", "value4" }));
        }

        [Test]
        public void GetInstanceVisitorGeneratorTests_ToGetListOfComplexAndSimpleValueAndNullValue()
        {
            var obj = new Test1()
            {
                List2 = new Test1[] {
                new Test1 { List = new List<string> { "value1", "value2" } },
                new Test1 { List = null } }
            };

            IGetInstanceVisitor<Test1> visitor = new GetInstanceVisitorGenerator<Test1>("List2.List");
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2" }));
        }
    }
}
