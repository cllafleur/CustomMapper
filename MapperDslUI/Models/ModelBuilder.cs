using MapperDslUI.Models.Origin;
using MapperDslUI.Models.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperDslUI.Models
{
    public class ModelBuilder
    {
        public static OriginObject GetNewOriginObject()
        {
            var origin = new OriginObject()
            {
                Title = "supertitle",
                Description = "superdescription",
                ModificationDate = new DateTime(2021, 07, 31),
            };
            return origin;
        }

        public static TargetObject GetNewTargetObject()
        {
            var target = new TargetObject() { Description = new DescriptionObject() };
            return target;
        }
    }
}
