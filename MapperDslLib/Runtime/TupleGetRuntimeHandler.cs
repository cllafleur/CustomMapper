using MapperDslLib.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapperDslLib.Runtime
{
    internal class TupleGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private List<IGetRuntimeHandler<TOrigin>> tupleParts;
        private ParsingInfo parsingInfo;

        public TupleGetRuntimeHandler(List<IGetRuntimeHandler<TOrigin>> tupleParts, ParsingInfo parsingInfo)
        {
            this.tupleParts = tupleParts;
            this.parsingInfo = parsingInfo;
        }

        public SourceResult Get(TOrigin obj)
        {
            var enumerators = new List<IEnumerator<object>>();
            var getResults = new List<SourceResult>();
            var tupleSources = new TupleSources();
            foreach (var part in tupleParts)
            {
                var result = part.Get(obj);
                if (result.IsLiteral) result.KeepEnumerate = true;
                getResults.Add(result);
                enumerators.Add(result.Result.GetEnumerator());
                tupleSources.Add(result);
            }
            return new TupleSourceResult()
            {
                Result = GetResults()
            };

            IEnumerable<TupleValues> GetResults()
            {
                var firstIteration = true;
                while (enumerators.Where(e => e != null).Any())
                {
                    TupleValues tuple = new TupleValues();
                    var allEmpty = true;

                    for (int i = 0; i < enumerators.Count; i++)
                    {
                        object value = null;
                        if (enumerators[i] != null)
                        {
                            if (enumerators[i].MoveNext())
                            {
                                value = enumerators[i].Current;
                                if (firstIteration || !getResults[i].IsLiteral) allEmpty = false;
                            }
                            else
                            {
                                enumerators[i] = null;
                            }
                        }
                        tuple.Add(value);
                    }

                    if (!allEmpty)
                    {
                        yield return tuple;
                    }
                    else
                    {
                        break;
                    }
                    firstIteration = false;
                }
            }
        }
    }
}
