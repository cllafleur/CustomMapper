using MapperDslLib.Runtime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperDslLib.Tests.Runtime
{
    public class InstanceVisitorTests
    {
        class Test1
        {
            public string Text { get; set; }

            public List<string> List { get; set; }

            public Test1[] List2 { get; set; }
        }

        [Test]
        public void InstanceVisitorGetInstance_ToGetScalarValue()
        {
            var obj = new Test1() { Text = "currentText" };

            InstanceVisitor<Test1> visitor = new InstanceVisitor<Test1>("Text", DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.FirstOrDefault(), Is.EqualTo("currentText"));
        }

        [Test]
        public void InstanceVisitorGetInstance_ToGetListOfSimpleValue()
        {
            var obj = new Test1() { List = new List<string>() { "value1", "value2" } };

            InstanceVisitor<Test1> visitor = new InstanceVisitor<Test1>("List", DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(obj.List));
        }

        [Test]
        public void InstanceVisitorGetInstance_ToGetListOfComplexValue()
        {
            var obj = new Test1() { List2 = new Test1[] { new Test1 { Text = "value1" }, new Test1 { Text = "value2" } } };

            InstanceVisitor<Test1> visitor = new InstanceVisitor<Test1>("List2.Text", DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2" }));
        }


        [Test]
        public void InstanceVisitorGetInstance_ToGetListOfComplexAndSimpleValue()
        {
            var obj = new Test1()
            {
                List2 = new Test1[] {
                new Test1 { List = new List<string> { "value1", "value2" } },
                new Test1 { List = new List<string>{ "value3", "value4" } } }
            };

            InstanceVisitor<Test1> visitor = new InstanceVisitor<Test1>("List2.List", DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2", "value3", "value4" }));
        }

        [Test]
        public void InstanceVisitorGetInstance_ToGetListOfComplexAndSimpleValueAndNullValue()
        {
            var obj = new Test1()
            {
                List2 = new Test1[] {
                new Test1 { List = new List<string> { "value1", "value2" } },
                new Test1 { List = null } }
            };

            InstanceVisitor<Test1> visitor = new InstanceVisitor<Test1>("List2.List", DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2" }));
        }
    }
}
