using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;
using NUnit.Framework;

namespace MapperDslLib.Tests.Runtime
{
    public class SetInstanceBuilderTests
    {
        class t { public tt Tt { get; set; } }
        class tt { public List<string> Items { get; } = new List<string>(); }

        [Test]
        public void InstanceVisitorBuilderSetInstance_SetEnumeratorWithFullList_Success()
        {
            var input = new[] { "item1", "item2" };

            var obj = new tt();

            ISetterAccessor visitor = InstanceVisitorBuilder.GetSetterAccessor<tt>(new[] { new FieldInstanceRefMapper("Items", null) }, DefaultPropertyResolverHandler.Instance);
            visitor.SetInstance(obj, input);

            Assert.That(obj.Items.Count(), Is.EqualTo(2));
            Assert.That(obj.Items, Is.EqualTo(input));
        }

        [Test]
        public void InstanceVisitorBuilderSetInstance_SetEnumeratorWithFullListPlus1Level_Success()
        {
            var input = new[] { "item1", "item2" };

            var obj = new t() { Tt = new tt() };

            ISetterAccessor visitor = InstanceVisitorBuilder.GetSetterAccessor<t>(new[] { new FieldInstanceRefMapper("Tt", null), new FieldInstanceRefMapper("Items", null) }, DefaultPropertyResolverHandler.Instance);
            visitor.SetInstance(obj, input);

            Assert.That(obj.Tt.Items.Count(), Is.EqualTo(2));
            Assert.That(obj.Tt.Items, Is.EqualTo(input));
        }
    }
}
