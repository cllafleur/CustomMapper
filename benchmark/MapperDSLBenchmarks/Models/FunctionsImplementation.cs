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
        public SourceResult GetObject(VacancyDetailRead instanceObj, Parameters parameters)
        {
            return new SourceResult()
            {
                Result = GetResults()
            };

            IEnumerable<object> GetResults()
            {
                foreach (var item in parameters.Values[0].Result)
                {
                    yield return $"{item}";
                }
            }
        }
    }

    class GenerateId : IExtractFunctionHandler<VacancyDetailRead>
    {
        public SourceResult GetObject(VacancyDetailRead instanceObj, Parameters parameters)
        {
            return new SourceResult()
            {
                Result = GetResults()
            };

            IEnumerable<object> GetResults()
            {
                var hash = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{parameters.Values[0].Result.FirstOrDefault()}_{DateTime.Now.ToString("s")}"))).Replace("-", "");
                yield return $"{parameters.Values[0].Result.FirstOrDefault()}_{hash.Substring(0, 8).ToLower()}";
            }
        }
    }

    class AddProperty : IInsertFunctionHandler<JobAd>, IInsertTupleFunctionHandler<JobAd>
    {
        public void SetObject(JobAd instanceObject, SourceResult source, Parameters parameters)
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
                    if (!instanceObject.Properties.ContainsKey((string)parameters.Values[0].Result.First()))
                    {
                        instanceObject.Properties.Add((string)parameters.Values[0].Result.First(), new SingleProperty() { Items = new List<SinglePropertyItem>() { property } });
                    }
                    else
                    {
                        ((SingleProperty)instanceObject.Properties[(string)parameters.Values[0].Result.First()]).Items.Add(property);
                    }
                }
            }
        }

        public void SetObject(JobAd instanceObject, TupleSourceResult source, Parameters parameters)
        {
            foreach (var tuple in source.Result)
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

                if (!instanceObject.Properties.ContainsKey((string)parameters.Values[0].Result.First()))
                {
                    instanceObject.Properties.Add((string)parameters.Values[0].Result.First(), property);
                }
                else
                {
                    switch (property)
                    {
                        case SingleProperty singleProperty:
                            ((SingleProperty)instanceObject.Properties[(string)parameters.Values[0].Result.First()]).Items.Add(singleProperty.Items[0]);
                            break;
                        case CompositeProperty compositeProperty:
                            ((CompositeProperty)instanceObject.Properties[(string)parameters.Values[0].Result.First()]).Items.Add(compositeProperty.Items[0]);
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
