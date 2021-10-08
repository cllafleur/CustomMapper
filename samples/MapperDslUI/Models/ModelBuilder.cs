using MapperDslUI.Models.Origin;
using MapperDslUI.Models.Target;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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

        public static VacancyDetailRead GetNewVacancyDetailRead()
        {
            using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(ModelBuilder).Namespace}.VacancyDetailRead.json"), Encoding.UTF8);
            var content = reader.ReadToEnd();
            var obj = JsonSerializer.Deserialize<VacancyDetailRead>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return obj;
        }

        public static TargetObject GetNewTargetObject()
        {
            var target = new TargetObject() { Description = new DescriptionObject() };
            return target;
        }

        public static JobAd GetNewJobAd()
        {
            return new JobAd();
        }
    }
}
