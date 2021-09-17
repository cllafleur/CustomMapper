using MapperDslLib.Runtime;
using MapperDslUI.Models.Origin;
using MapperDslUI.Models.Target;
using MapperDslUI.Models.Target.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MapperDSLBenchmarks.Models
{
    class ExtractRef : IExtractFunctionHandler<VacancyDetailRead>
    {
        public SourceResult GetObject(VacancyDetailRead instanceObj, DataSourceInfo originInfo, IEnumerable<object>[] args)
        {
            return new SourceResult()
            {
                Result = GetResults()
            };

            IEnumerable<object> GetResults()
            {
                foreach (var item in args[0])
                {
                    yield return $"{item}";
                }
            }
        }
    }

    class GenerateId : IExtractFunctionHandler<VacancyDetailRead>
    {
        public SourceResult GetObject(VacancyDetailRead instanceObj, DataSourceInfo originInfo, IEnumerable<object>[] args)
        {
            return new SourceResult()
            {
                Result = GetResults()
            };

            IEnumerable<object> GetResults()
            {
                var hash = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{args[0].FirstOrDefault()}_{DateTime.Now.ToString("s")}"))).Replace("-", "");
                yield return $"{args[0].FirstOrDefault()}_{hash.Substring(0, 8).ToLower()}";
            }
        }
    }

    class AddProperty : IInsertFunctionHandler<JobAd>, IInsertTupleFunctionHandler<JobAd>
    {
        public void SetObject(JobAd instanceObject, SourceResult source, params object[] args)
        {
            foreach (var item in source.Result)
            {
                SinglePropertyItem property = null;
                switch (item)
                {
                    case Reference reference:
                        property = new SinglePropertyItem { Id = reference.Id, Label = reference.Label };
                        break;
                    default:
                        property = new SinglePropertyItem() { Label = item.ToString() };
                        break;
                }
                if (property != null)
                {
                    if (!instanceObject.Properties.ContainsKey((string)args[0]))
                    {
                        instanceObject.Properties.Add((string)args[0], new SingleProperty() { Items = new List<SinglePropertyItem>() { property } });
                    }
                    else
                    {
                        ((SingleProperty)instanceObject.Properties[(string)args[0]]).Items.Add(property);
                    }
                }
            }
        }

        public void SetObject(JobAd instanceObject, DataSourceInfo originInfo, IEnumerable<IEnumerable<object>> value, params object[] args)
        {
            foreach (var tuple in value)
            {
                var tupleValues = new List<object>();
                foreach (var item in tuple)
                {
                    tupleValues.Add(item);
                }
                Property property = tupleValues.Count switch
                {
                    1 => new SingleProperty() { Items = new List<SinglePropertyItem>() { BuildSingleProperty(tupleValues[0]) } },
                    > 1 => BuildCompositeItem(tupleValues),
                    _ => null,
                };

                if (property == null) continue;

                if (!instanceObject.Properties.ContainsKey((string)args[0]))
                {
                    instanceObject.Properties.Add((string)args[0], property);
                }
                else
                {
                    switch (property)
                    {
                        case SingleProperty singleProperty:
                            ((SingleProperty)instanceObject.Properties[(string)args[0]]).Items.Add(singleProperty.Items[0]);
                            break;
                        case CompositeProperty compositeProperty:
                            ((CompositeProperty)instanceObject.Properties[(string)args[0]]).Items.Add(compositeProperty.Items[0]);
                            break;
                    }

                }
            }
            CompositeProperty BuildCompositeItem(List<object> tuple)
            {
                CompositePropertyItem property = new CompositePropertyItem();
                foreach (var item in tuple)
                {
                    property.Add(BuildSingleProperty(item));
                }
                return new CompositeProperty() { Items = new List<CompositePropertyItem>() { property } };
            }
            SinglePropertyItem BuildSingleProperty(object obj)
            {
                return obj switch
                {
                    Reference reference => new SinglePropertyItem { Id = reference.Id, Label = reference.Label },
                    _ => new SinglePropertyItem() { Label = obj?.ToString() }
                };
            }
        }
    }
}
