using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperDslLib.Tests.Runtime
{
    public class GetInstanceVisitorBuilderTests
    {
        class Test1
        {
            public string Text { get; set; }

            public List<string> List { get; set; }

            public Test1[] List2 { get; set; }

            public Test1 Test { get; set; }
        }

        class Test2
        {
            public Dictionary<string, string> Dic { get; set; } = new Dictionary<string, string>();
        }

        [Test]
        public void InstanceVisitorBuilderGetInstance_ToGetScalarValue()
        {
            var obj = new Test1() { Text = "currentText" };

            IGetterAccessor visitor = InstanceVisitorBuilder.GetGetterAccessor<Test1>(new[] { new FieldInstanceRefMapper("Text", null) }, DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.FirstOrDefault(), Is.EqualTo("currentText"));
        }

        [Test]
        public void InstanceVisitorBuilderGetInstance_ToGetListOfSimpleValue()
        {
            var obj = new Test1() { List = new List<string>() { "value1", "value2" } };

            IGetterAccessor visitor = InstanceVisitorBuilder.GetGetterAccessor<Test1>(new[] { new FieldInstanceRefMapper("List", null) }, DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(obj.List));
        }

        [Test]
        public void InstanceVisitorBuilderGetInstance_ToGetListOfComplexValue()
        {
            var obj = new Test1() { List2 = new Test1[] { new Test1 { Text = "value1" }, new Test1 { Text = "value2" } } };

            IGetterAccessor visitor = InstanceVisitorBuilder.GetGetterAccessor<Test1>(new[] { new FieldInstanceRefMapper("List2", null), new FieldInstanceRefMapper("Text", null) }, DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2" }));
        }

        [Test]
        public void InstanceVisitorBuilderGetInstance_ToGetListOfComplexValue2()
        {
            var obj = new Test1() { Test = new Test1 { List2 = new Test1[] { new Test1 { Text = "value1" }, new Test1 { Text = "value2" } } } };

            IGetterAccessor visitor = InstanceVisitorBuilder.GetGetterAccessor<Test1>(new[] { new FieldInstanceRefMapper("Test", null), new FieldInstanceRefMapper("List2", null), new FieldInstanceRefMapper("Text", null) }, DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2" }));
        }

        [Test]
        public void InstanceVisitorBuilderGetInstance_ToGetListOfComplexAndSimpleValue()
        {
            var obj = new Test1()
            {
                List2 = new Test1[] {
                new Test1 { List = new List<string> { "value1", "value2" } },
                new Test1 { List = new List<string>{ "value3", "value4" } } }
            };

            IGetterAccessor visitor = InstanceVisitorBuilder.GetGetterAccessor<Test1>(new[] { new FieldInstanceRefMapper("List2", null), new FieldInstanceRefMapper("List", null) }, DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2", "value3", "value4" }));
        }

        [Test]
        public void InstanceVisitorBuilderGetInstance_ToGetListOfComplexAndSimpleValueAndNullValue()
        {
            var obj = new Test1()
            {
                List2 = new Test1[] {
                new Test1 { List = new List<string> { "value1", "value2" } },
                new Test1 { List = null } }
            };

            IGetterAccessor visitor = InstanceVisitorBuilder.GetGetterAccessor<Test1>(new[] { new FieldInstanceRefMapper("List2", null), new FieldInstanceRefMapper("List", null) }, DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(new[] { "value1", "value2" }));
        }

        [Test]
        public void InstanceVisitorBuilderGetInstance_RetrievePropertiesFromDictionary()
        {
            var obj = new Test2() { Dic = { { "key1", "val1" } } };

            IGetterAccessor visitor = InstanceVisitorBuilder.GetGetterAccessor<Test2>(new[] { new FieldInstanceRefMapper("Dic", null), new FieldInstanceRefMapper("key1", null) }, DefaultPropertyResolverHandler.Instance);
            var result = visitor.GetInstance(obj);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(new[] { "val1" }));
        }
    }
}
